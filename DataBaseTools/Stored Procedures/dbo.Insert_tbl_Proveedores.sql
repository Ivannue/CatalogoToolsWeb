CREATE PROCEDURE dbo.Insert_tbl_Proveedores
    @nNombre VARCHAR(150),
    @iLogo VARBINARY(MAX) = NULL,
    @bActivo BIT = 1,
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.tbl_Proveedores (nNombre, iLogo, bActivo)
    VALUES (@nNombre, @iLogo, @bActivo);

    SET @NewId = CAST(SCOPE_IDENTITY() AS INT);

    -- Opcional: devolver la fila insertada
    SELECT * FROM dbo.tbl_Proveedores WHERE nId = @NewId;
END;