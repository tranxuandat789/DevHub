# Implementation Plan: VAT Tax & Digital Bill (Hóa đơn điện tử)

**Tác giả:** AnhPT
**Ngày:** 09/07/2026
**Phạm vi:** Tính thuế GTGT 8% trên giá trị dịch vụ cuối cùng (sau khuyến mại/khấu trừ nâng cấp) và bổ sung mã số thuế doanh nghiệp khi xuất hóa đơn.
**Nguyên tắc:** Additive only — không đổi ý nghĩa các cột hiện có (`final_amount` vẫn là giá trị trước thuế), chỉ thêm cột/field mới.

---

## 0. Quyết định thiết kế cần chốt trước khi code

- [ ] **Nguồn lấy mã số thuế người mua (`BuyerTaxCode`)**: hiện tại `TaxCode` nằm trên `Recruiter` (chưa có `Company` entity riêng theo ghi nhận trước đó). Cần xác nhận: lấy trực tiếp từ `Recruiter.TaxCode` hay đã tách sang bảng `Company`? → Ảnh hưởng chữ ký hàm ở Repository layer (mục 3).
- [ ] **Làm tròn thuế**: chốt quy tắc `MidpointRounding.AwayFromZero`, làm tròn đến đơn vị đồng (không số lẻ) — khớp cách VNPay yêu cầu `vnp_Amount` là số nguyên.
- [ ] **Thuế suất đọc từ config** (`Tax:VatRatePercent`), không hardcode — vì mức 8% chỉ là ưu đãi tạm thời đến 31/12/2026 theo NĐ 174/2025/NĐ-CP, sau đó có thể về lại 10%.

---

## 1. Database Schema (additive — ALTER TABLE)

**File:** `Database_Schema.sql`

- [ ] Thêm 4 cột vào `package_transaction`:
  ```sql
  ALTER TABLE [package_transaction] ADD
      [vat_rate] DECIMAL(5,2) NOT NULL DEFAULT 8,
      [vat_amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
      [total_amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
      [buyer_tax_code] NVARCHAR(50) NULL;
  ```
- [ ] Giữ nguyên `final_amount` = giá trị dịch vụ **trước thuế** (sau discount + deduction). Không đổi ý nghĩa cột này để tránh vỡ các báo cáo/lịch sử đã có.
- [ ] `total_amount` = số tiền **thực gửi qua VNPay** = `final_amount + vat_amount`.
- [ ] Cân nhắc thêm index nếu sau này cần báo cáo thuế theo kỳ: `CREATE NONCLUSTERED INDEX [idx_payment_transaction_date] ON [package_transaction]([transaction_date]);` (không bắt buộc ở giai đoạn này).

## 2. Model

**File:** `PackageTransaction.cs`

- [ ] Thêm 4 property tương ứng cột mới:
  ```csharp
  public decimal VatRate { get; set; }
  public decimal VatAmount { get; set; }
  public decimal TotalAmount { get; set; }
  public string? BuyerTaxCode { get; set; }
  ```

## 3. Repository

**File:** `IPaymentRepository` (interface) + implementation

- [ ] Thêm method lấy mã số thuế người mua tại thời điểm giao dịch:
  ```csharp
  Task<string?> GetBuyerTaxCodeAsync(int companyId);
  ```
  - Nếu vẫn dùng `Recruiter.TaxCode`: query theo `companyId` tương ứng recruiter đang đăng nhập.
  - Nếu đã có `Company` entity: đổi tham số/nguồn query cho khớp — **chờ xác nhận mục 0**.
- [ ] Không cần thay đổi các hàm ghi/đọc `PackageTransaction` khác (`CreateTransactionAsync`, `GetByTxnRefAsync`, `MarkPaidAndActivateAsync`...) — EF Core tự map thêm 4 field mới miễn `PackageTransaction` model đã cập nhật.

## 4. Configuration

**File:** `appsettings.json`

- [ ] Thêm section:
  ```json
  "Tax": {
    "VatRatePercent": 8
  }
  ```

## 5. ViewModels

**File:** `PaymentViewModels.cs`

- [ ] `VoucherCheckResultVm` — thêm:
  ```csharp
  public decimal VatRate { get; set; }
  public decimal VatAmount { get; set; }
  public decimal TotalAmount { get; set; }
  ```
- [ ] `PaymentHistoryDetailVm` — thêm:
  ```csharp
  public decimal VatRate { get; set; }
  public decimal VatAmount { get; set; }
  public decimal TotalAmount { get; set; }
  public string? BuyerTaxCode { get; set; }
  ```
- [ ] `PaymentHistoryItemVm` (danh sách lịch sử) — cân nhắc thêm `TotalAmount` nếu muốn hiển thị "đã gồm VAT" ngay ở bảng danh sách (không bắt buộc, tuỳ UI).

## 6. Service — `PaymentService.cs`

- [ ] Inject `IConfiguration` đã có sẵn — không cần thêm dependency mới.
- [ ] **`ValidateVoucherAsync`**: sau khi tính `finalAmount`, tính thêm:
  ```csharp
  decimal vatRate = _configuration.GetValue<decimal>("Tax:VatRatePercent", 8m);
  decimal vatAmount = Math.Round(finalAmount * vatRate / 100m, 0, MidpointRounding.AwayFromZero);
  decimal totalAmount = finalAmount + vatAmount;
  ```
  Gán vào `VoucherCheckResultVm` (VatRate, VatAmount, TotalAmount).
- [ ] **`CreatePaymentUrlAsync`**:
  - [ ] Gọi `_paymentRepo.GetBuyerTaxCodeAsync(companyId)` để snapshot mã số thuế trước khi tạo `tx`.
  - [ ] Gán `VatRate`, `VatAmount`, `TotalAmount`, `BuyerTaxCode` vào entity `PackageTransaction` mới tạo.
  - [ ] **Đổi điều kiện free-upgrade**: từ `voucherCheck.FinalAmount <= 0` → `voucherCheck.TotalAmount <= 0` (vì khi giảm giá 100%, VAT trên 0 đồng cũng là 0, giữ logic nhất quán).
  - [ ] **Đổi số tiền gửi VNPay**: `var amount = (long)(voucherCheck.TotalAmount * 100);` — **không còn dùng `FinalAmount`**.
- [ ] **`ConfirmAsync`**: đổi điều kiện verify số tiền:
  ```csharp
  if (vnpAmount != (long)(tx.TotalAmount * 100))
      return (false, "04", "Số tiền không hợp lệ");
  ```
  ⚠️ Đây là điểm dễ bỏ sót nhất — nếu quên đổi, mọi giao dịch có VAT sẽ bị reject do so sánh với `FinalAmount` (số tiền chưa gồm thuế).
- [ ] **`GetHistoryDetailAsync`**: map thêm `VatRate`, `VatAmount`, `TotalAmount`, `BuyerTaxCode` vào `PaymentHistoryDetailVm`.
- [ ] **`ExportHistoryAsync`**: thêm 2 cột "Thuế GTGT (VND)" và "Tổng thanh toán (VND)" vào file Excel xuất ra (tuỳ chọn, nên làm để đồng bộ với hóa đơn).

## 7. Controller — `RecruiterPaymentController.cs`

- [ ] Không cần thay đổi signature action nào — toàn bộ logic tính thuế nằm trong Service, Controller chỉ truyền/nhận VM như hiện tại.
- [ ] Kiểm tra `CheckVoucher` action: response JSON trả về giờ có thêm `VatRate/VatAmount/TotalAmount` — đảm bảo phía JS (Subscription page) đọc đúng field mới.

## 8. View

**File:** `HistoryDetails.cshtml` (và `Subscription.cshtml` cho phần hiển thị trước khi thanh toán)

- [ ] Thêm block hiển thị breakdown theo chuẩn hóa đơn VAT:
  ```
  Giá trị dịch vụ (đã trừ khuyến mại):  {FinalAmount} đ
  Thuế GTGT ({VatRate}%):                {VatAmount} đ
  ─────────────────────────────────────
  Tổng thanh toán:                      {TotalAmount} đ
  Mã số thuế:                           {BuyerTaxCode}
  ```
- [ ] `Subscription.cshtml` — sau khi gọi `CheckVoucher` (AJAX), cập nhật UI hiển thị số tiền cuối cùng đã gồm VAT trước khi khách nhấn "Thanh toán" (tránh khách bị bất ngờ vì số tiền VNPay yêu cầu khác với số hiển thị ban đầu).

## 9. Kiểm thử

- [ ] Unit test `ValidateVoucherAsync`: kiểm tra `VatAmount` tính đúng theo `VatRate` config, làm tròn đúng quy tắc.
- [ ] Unit test `CreatePaymentUrlAsync`: `vnp_Amount` gửi đi phải bằng `TotalAmount * 100`, không phải `FinalAmount * 100`.
- [ ] Unit test `ConfirmAsync`: giả lập VNPay trả về đúng `TotalAmount` → pass; trả về `FinalAmount` (thiếu thuế) → phải bị reject mã lỗi `04`.
- [ ] Test case giảm giá 100%: `TotalAmount = 0` → nhánh free-upgrade phải kích hoạt đúng, không tạo VNPay URL.
- [ ] Test trường hợp `BuyerTaxCode` null (recruiter chưa cập nhật mã số thuế) — xác nhận hành vi mong muốn: chặn thanh toán hay cho phép xuất hóa đơn thiếu mã số thuế? (cần chốt ở mục 0, bổ sung nếu quyết định chặn).

## 10. Rà soát dữ liệu cũ (migration data)

- [ ] Các giao dịch `PackageTransaction` đã tồn tại trước khi thêm cột (status = SUCCESS) sẽ có `vat_rate/vat_amount/total_amount` = giá trị DEFAULT (8 / 0 / 0) — cần quyết định: để nguyên (vì đã hoàn tất, không xuất hóa đơn lại) hay chạy script backfill ước tính theo `final_amount` hiện có. Khuyến nghị: **để nguyên**, ghi chú rõ trong tài liệu là áp dụng từ ngày triển khai trở đi, không hồi tố.

---

## Tóm tắt thứ tự triển khai

1. Chốt mục 0 (đặc biệt là nguồn `BuyerTaxCode`)
2. Schema + Model
3. Config
4. Repository (thêm 1 hàm)
5. ViewModels
6. Service (4 hàm: ValidateVoucherAsync, CreatePaymentUrlAsync, ConfirmAsync, GetHistoryDetailAsync)
7. View
8. Test
