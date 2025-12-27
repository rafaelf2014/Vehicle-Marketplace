insert into Classe (Nome) values
('SUV'),
('Compacto'),
('Sedan'),
('Hatchback'),
('Carrinha'),
('Desportivo'),
('Pickup'),
('Elétrico');

insert into Combustivel (Tipo) values
('Gasolina'),
('Diesel'),
('Elétrico'),
('Híbrido'),
('GPL');

insert into Marca(Nome) values
('Honda'),
('Toyota'),
('BMW'),
('Mercedes'),
('Volkswagen');

INSERT INTO Modelo (Nome, ID_Marca) VALUES
('Civic', 1),
('Accord', 1),
('Corolla', 2),
('Yaris', 2),
('320d', 3),
('X5', 3),
('A-Class', 4),
('C-Class', 4),
('Golf', 5),
('Polo', 5);

INSERT INTO Localizacao (Distrito) VALUES
('Aveiro'),
('Beja'),
('Braga'),
('Bragança'),
('Castelo Branco'),
('Coimbra'),
('Évora'),
('Faro'),
('Guarda'),
('Leiria'),
('Lisboa'),
('Portalegre'),
('Porto'),
('Santarém'),
('Setúbal'),
('Viana do Castelo'),
('Vila Real'),
('Viseu'),
('Madeira'),
('Açores');

   --   ALTER TABLE Veiculo
   --ADD IdMarca INT;

--   ALTER TABLE Veiculo
--   ADD Caixa VarChar(1);
--   --FOREIGN KEY (IdMarca) REFERENCES Marca(ID_Marca);
--<<<<<<< HEAD

--   --Alter table veiculo
--   --add Caixa varchar(1);

   
   alter table Anuncio
add NVisitas int not null default 0;
   
--=======
-->>>>>>> origin/AlexandrePereira-Branch
   

   