CREATE TABLE developers (
    dev_id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL
);

CREATE TABLE scripttypelookup (
    type_id INT IDENTITY(1,1) PRIMARY KEY,
    type VARCHAR(50) NOT NULL
);

CREATE TABLE scripts (
    script_id INT IDENTITY(1,1) PRIMARY KEY,
    dev_id INT NOT NULL FOREIGN KEY REFERENCES developers(dev_id),
    script_name VARCHAR(50) NOT NULL,
    script_s3_uri VARCHAR(100) NOT NULL,
    script_type INT NOT NULL FOREIGN KEY REFERENCES scripttypelookup(type_id),
    script_version INT NOT NULL DEFAULT 1,
    last_updated DATETIME DEFAULT GETDATE()
);
