-- =============================================================================
-- Plataforma de Gestao de Talentos IT - Grupo BC5
-- Script de dados iniciais (seed)
-- =============================================================================
-- Insere os dados base necessarios ao arranque do sistema:
--   - 4 Areas profissionais
--   - 1 utilizador Admin (admin / admin123) com a conta ja activa
--
-- Pre-requisito: ter executado primeiro o schema.sql.
-- Para correr: psql -d TalentosDB -f database/seed.sql
-- =============================================================================

-- -----------------------------------------------------------------------------
-- Areas profissionais
-- -----------------------------------------------------------------------------
-- Os IDs sao fixos para corresponder aos seeds da migration InitialCreate
-- do Entity Framework. O ON CONFLICT permite re-correr o script sem erros.

INSERT INTO "Areas" ("Id", "Nome", "CriadoEm") VALUES
    (1, 'Developer',       '2026-05-06 18:04:23+00'),
    (2, 'Designer',        '2026-05-06 18:04:23+00'),
    (3, 'Product Manager', '2026-05-06 18:04:23+00'),
    (4, 'Project Manager', '2026-05-06 18:04:23+00')
ON CONFLICT ("Id") DO NOTHING;

-- Reposiciona a sequencia para evitar conflitos em inserts futuros
SELECT setval(
    pg_get_serial_sequence('"Areas"', 'Id'),
    GREATEST((SELECT MAX("Id") FROM "Areas"), 1)
);

-- -----------------------------------------------------------------------------
-- Utilizador Admin
-- -----------------------------------------------------------------------------
-- Username: admin
-- Password: admin123  (hash BCrypt)
-- Role:     2 (Admin)
-- EstadoConta: 1 (Ativo)
-- O hash abaixo foi gerado com BCrypt (cost 11) e e compativel com
-- BCrypt.Net.BCrypt.Verify do back-end.

INSERT INTO "Users" ("Username", "Password", "Role", "EstadoConta")
SELECT 'admin',
       '$2b$11$/m.PT3.IcpGFLs5Mc4.5dOyVDsZBzQH8kmBL/MBby/2.H2emxTKJ2',
       2,
       1
WHERE NOT EXISTS (
    SELECT 1 FROM "Users" WHERE "Username" = 'admin'
);

-- =============================================================================
-- Fim do seed
-- =============================================================================
