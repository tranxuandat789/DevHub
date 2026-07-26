ALTER TABLE province ADD display_order INT NOT NULL DEFAULT 999;
GO
UPDATE province SET display_order = 999;
UPDATE province SET display_order = 1 WHERE province_name = N'Hà Nội';
UPDATE province SET display_order = 2 WHERE province_name = N'Hồ Chí Minh';
UPDATE province SET display_order = 3 WHERE province_name = N'Đà Nẵng';
UPDATE province SET display_order = 4 WHERE province_name = N'Hải Phòng';
UPDATE province SET display_order = 5 WHERE province_name = N'Cần Thơ';
GO
