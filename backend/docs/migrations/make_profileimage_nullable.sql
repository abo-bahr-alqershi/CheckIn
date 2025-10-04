-- Migration: Make ProfileImage column nullable
-- Date: 2025-10-01
-- Description: يجعل عمود ProfileImage في جدول Users يقبل القيم NULL

USE [db_abd8fd_bookn2];
GO

-- التحقق من وجود الجدول
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    -- التحقق من وجود العمود
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'ProfileImage')
    BEGIN
        -- تحديث السجلات الموجودة التي تحتوي على NULL إلى string فارغ
        UPDATE [dbo].[Users]
        SET [ProfileImage] = ''
        WHERE [ProfileImage] IS NULL;
        
        -- تعديل العمود ليصبح nullable
        ALTER TABLE [dbo].[Users]
        ALTER COLUMN [ProfileImage] NVARCHAR(500) NULL;
        
        PRINT 'تم تعديل عمود ProfileImage بنجاح ليصبح nullable';
    END
    ELSE
    BEGIN
        PRINT 'العمود ProfileImage غير موجود في جدول Users';
    END
END
ELSE
BEGIN
    PRINT 'جدول Users غير موجود';
END
GO
