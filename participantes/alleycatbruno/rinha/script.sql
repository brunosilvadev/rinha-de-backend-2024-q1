CREATE UNLOGGED TABLE "Clientes" (
    "Id" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE),
	"Limite" INT4 NULL,
    "Saldo" INT8  NULL,
    CONSTRAINT "PK_Clientes" PRIMARY KEY ("Id")
);

INSERT INTO "Clientes" ("Id","Limite","Saldo") VALUES
	 (1, 1000 * -100,0),
	 (2, 800 * -100,0),
	 (3, 10000 * -100,0),
	 (4, 100000 * -100,0),
	 (5, 5000 * -100,0);

CREATE UNLOGGED TABLE "Transacoes" (
    "TransacaoId" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE),
	"Valor" INT8 NOT NULL,
    "Tipo" varchar(1) NULL,
    "Descricao" varchar(100) NULL,
    "Realizada_em" TIMESTAMP NULL,
    "ClienteId" int4 NULL,
    FOREIGN KEY ("ClienteId") REFERENCES "Clientes"("Id"),
    CONSTRAINT "FK_Transacoes_Clientes_ClienteId" FOREIGN KEY ("ClienteId") REFERENCES public."Clientes"("Id")
);

CREATE INDEX "IX_Transacoes_ClienteId" ON public."Transacoes" USING btree ("ClienteId");

CREATE FUNCTION credit(cliente INTEGER, valor int8, descricao varchar(15))
RETURNS int8 
LANGUAGE plpgsql
AS $$
DECLARE
    Saldo int8;
BEGIN
  UPDATE "Clientes"
  SET "Saldo" = "Saldo" + valor
  WHERE "Id" = cliente
  RETURNING "Saldo" into Saldo;

  INSERT INTO "Transacoes" ("Valor", "Tipo", "Descricao", "Realizada_em", "ClienteId")
  VALUES( valor, 'c', descricao, now(), cliente);

  RETURN Saldo;
END;
$$;

CREATE FUNCTION debit(cliente INTEGER, valor int8, descricao varchar(15))
RETURNS int8 
LANGUAGE plpgsql
AS $$
DECLARE
    Saldo int8;
BEGIN
    UPDATE "Clientes"
    SET "Saldo" = "Saldo"  - valor
    WHERE "Id" = cliente
    AND "Saldo" - valor >= "Limite"
    RETURNING "Saldo" into Saldo;
   
    IF Saldo IS NOT NULL THEN
		INSERT INTO "Transacoes" ("Valor", "Tipo", "Descricao", "Realizada_em", "ClienteId")
		VALUES( valor, 'd', descricao, now(), cliente);
    END IF;
    
    RETURN Saldo;
END;
$$;

