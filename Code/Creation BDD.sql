-- DROP DATABASE MaSociete
CREATE DATABASE MaSociete
GO

USE MaSociete
GO
-- DROP TABLE TBL_Services
CREATE TABLE TBL_Services (
	PK_Service INT PRIMARY KEY IDENTITY(1,1),
	Libele NVARCHAR(200),
	--FK_Chef_Service UNIQUEIDENTIFIER NULL
	)

-- DROP TABLE TBL_Employes
CREATE TABLE TBL_Employes(
	PK_Employe UNIQUEIDENTIFIER PRIMARY KEY DEFAULT newid(),
	Titre SMALLINT NULL,
	Code CHAR(5) NOT NULL,
	Nom NVARCHAR(50) NOT NULL,
	Prenom NVARCHAR(50) NOT NULL,
	DateEntree DATE NOT NULL DEFAULT Getdate(),
	DateSortie DATE NULL,
	FK_Service INT NOT NULL FOREIGN KEY REFERENCES TBL_Services(PK_Service),
	Salaire DECIMAL(18,2)
)
-- ALTER TABLE TBL_Services DROP COLUMN FK_Chef_Service
-- ALTER TABLE TBL_Services DROP CONSTRAINT FK__TBL_Servi__FK_Ch__3D5E1FD2
ALTER TABLE TBL_Services
	ADD  FK_Chef_Service  UNIQUEIDENTIFIER NULL
		 FOREIGN KEY  REFERENCES TBL_Employes(PK_Employe)



INSERT INTO TBL_Services(Libele)
VALUES ('Direction'),('Comptabilite'),('Informatique')



INSERT INTO TBL_Employes(Titre,Code,Nom, Prenom, FK_Service,Salaire)
VALUES
(1,'AG001','Gates','Bill',1,100000),
(1,'AG002','Altman','Sam',3,100000),
(2,'AG003','Von Der Layen','Maria',2,100000)