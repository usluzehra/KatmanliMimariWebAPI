-- Books & Authors: ImageFileKey ekle (yoksa)
IF COL_LENGTH('dbo.Books', 'ImageFileKey') IS NULL
    ALTER TABLE dbo.Books   ADD ImageFileKey NVARCHAR(64) NULL;

IF COL_LENGTH('dbo.Authors', 'ImageFileKey') IS NULL
    ALTER TABLE dbo.Authors ADD ImageFileKey NVARCHAR(64) NULL;

-- UploadImages tablosu yoksa oluştur
IF OBJECT_ID('dbo.UploadImages') IS NULL
BEGIN
    CREATE TABLE dbo.UploadImages(
        FileKey     NVARCHAR(64) NOT NULL UNIQUE,
        Base64Data  NVARCHAR(MAX) NOT NULL,
        ResimYolu   VARCHAR(512) NULL,
    );
END;
