ALTER TABLE province ADD region NVARCHAR(50) NULL;
GO
UPDATE province SET region = 'North' WHERE province_name IN (N'Hà Nội', N'Vĩnh Phúc', N'Bắc Ninh', N'Hà Nam', N'Hải Dương', N'Hưng Yên', N'Hải Phòng', N'Nam Định', N'Ninh Bình', N'Thái Bình', N'Hà Giang', N'Cao Bằng', N'Bắc Kạn', N'Lạng Sơn', N'Tuyên Quang', N'Thái Nguyên', N'Phú Thọ', N'Bắc Giang', N'Quảng Ninh', N'Lào Cai', N'Yên Bái', N'Điện Biên', N'Hòa Bình', N'Lai Châu', N'Sơn La');
UPDATE province SET region = 'Central' WHERE province_name IN (N'Thanh Hóa', N'Nghệ An', N'Hà Tĩnh', N'Quảng Bình', N'Quảng Trị', N'Thừa Thiên Huế', N'Đà Nẵng', N'Quảng Nam', N'Quảng Ngãi', N'Bình Định', N'Phú Yên', N'Khánh Hòa', N'Ninh Thuận', N'Bình Thuận', N'Kon Tum', N'Gia Lai', N'Đắk Lắk', N'Đắk Nông', N'Lâm Đồng');
UPDATE province SET region = 'South' WHERE province_name IN (N'Bình Phước', N'Bình Dương', N'Đồng Nai', N'Tây Ninh', N'Bà Rịa - Vũng Tàu', N'Hồ Chí Minh', N'Long An', N'Đồng Tháp', N'Tiền Giang', N'An Giang', N'Bến Tre', N'Vĩnh Long', N'Trà Vinh', N'Hậu Giang', N'Kiên Giang', N'Sóc Trăng', N'Bạc Liêu', N'Cà Mau', N'Cần Thơ');
GO
