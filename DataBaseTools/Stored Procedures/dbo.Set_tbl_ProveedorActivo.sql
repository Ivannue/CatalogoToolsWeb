CREATE PROCEDURE dbo.Set_tbl_ProveedorActivo
    @Id INT,
    @bActivo BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.tbl_Proveedores
    SET bActivo = @bActivo
    WHERE nId = @Id;

    SELECT @Id AS UpdatedId;
END;
GO