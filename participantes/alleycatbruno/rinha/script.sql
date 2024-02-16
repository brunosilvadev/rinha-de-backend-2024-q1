-- Create the Cliente table
CREATE TABLE "Clientes" (
    "Id" SERIAL PRIMARY KEY,
    "Limite" DECIMAL NOT NULL,
    "Saldo" DECIMAL NOT NULL
);

-- Create the Transacao table
CREATE TABLE "Transacoes" (
    "TransacaoId" SERIAL PRIMARY KEY,
    "Valor" DECIMAL NOT NULL,
    "Tipo" CHAR(1) CHECK ("Tipo" IN ('d', 'c')) NOT NULL,
    "Descricao" TEXT,
    "Realizada_em" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    "ClienteId" INTEGER NOT NULL,
    FOREIGN KEY ("ClienteId") REFERENCES "Clientes"("Id")
);

-- Indexes for faster search operations
CREATE INDEX idx_cliente_id ON "Clientes"("Id");
CREATE INDEX idx_transacao_clienteid ON "Transacoes"("ClienteId");

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