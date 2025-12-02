CREATE TABLE tbl_Productos (
    nId INT IDENTITY(1,1) PRIMARY KEY,
    nNombre VARCHAR(150) NOT NULL,
    iImage image,
    dFechaInsercion DATETIME NOT NULL DEFAULT GETDATE(),
    bActivo bit NOT NULL DEFAULT 1
);
SELECT * FROM tbl_Productos;
