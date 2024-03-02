-- Create the Cliente table
CREATE UNLOGGED TABLE "Clientes" (
    "Id" SERIAL PRIMARY KEY,
    "Limite" INT8 NOT NULL,
    "Saldo" INT8 NOT NULL
);

-- Create the Transacao table
CREATE UNLOGGED TABLE "Transacoes" (
    "TransacaoId" SERIAL PRIMARY KEY,
    "Valor" INT8 NOT NULL,
    "Tipo" CHAR(1) CHECK ("Tipo" IN ('d', 'c')) NOT NULL,
    "Descricao" CHAR(15),
    "Realizada_em" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    "ClienteId" INTEGER NOT NULL,
    FOREIGN KEY ("ClienteId") REFERENCES "Clientes"("Id")
);

CREATE OR REPLACE FUNCTION  credit(cliente INTEGER, valor int8, descricao varchar(15))
RETURNS int8 
LANGUAGE plpgsql
AS $$
DECLARE
    Saldo int8;
BEGIN
  UPDATE "Clientes" SET "Saldo" = "Saldo" + valor WHERE "Id" = cliente
  RETURNING "Saldo" into Saldo;

  INSERT INTO "Transacoes" ("Valor", "Tipo", "Descricao", "Realizada_em", "ClienteId")
  VALUES( valor, 'c', descricao, now(), cliente);

  RETURN Saldo;
END;
$$;

CREATE OR REPLACE FUNCTION  debit(cliente INTEGER, valor int8, descricao varchar(15))
RETURNS int8 
LANGUAGE plpgsql
AS $$
DECLARE
    Saldo int8;
BEGIN
    UPDATE "Clientes" SET "Saldo" = "Saldo"  - valor WHERE "Id" = cliente AND "Saldo" - valor >= "Limite"
    RETURNING "Saldo" into Saldo;
   IF Saldo IS NOT NULL THEN
		INSERT INTO "Transacoes" ("Valor", "Tipo", "Descricao", "Realizada_em", "ClienteId")
		VALUES( valor, 'd', descricao, now(), cliente);
   END IF;
   RETURN Saldo;
END;
$$;

-- Indexes for faster search operations
CREATE INDEX idx_cliente_id ON "Clientes"("Id");
CREATE INDEX idx_transacao_clienteid ON "Transacoes" USING btree ("ClienteId");

DO $$
BEGIN
  INSERT INTO "Clientes" ("Limite", "Saldo")
  VALUES
    (1000 * 100,0),
    (800 * 100,0),
    (10000 * 100,0),
    (100000 * 100,0),
    (5000 * 100,0);
END; $$

