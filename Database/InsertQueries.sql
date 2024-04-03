-- Insert data into ScriptType
INSERT INTO ScriptType (TypeName) VALUES ('Bash'), ('Python'), ('JavaScript');

-- Insert data into Developers
INSERT INTO Developers (Username) VALUES ('developer1'), ('developer2'), ('developer3');

-- Insert data into Scripts
INSERT INTO Scripts (DevId, TypeId, ScriptName, ScriptS3Url) VALUES 
    (1, 1, 'script1', 's3://bucket/script1'),
    (2, 2, 'script2', 's3://bucket/script2'),
    (3, 1, 'script3', 's3://bucket/script3'),
    (1, 2, 'script4', 's3://bucket/script4'),
    (2, 3, 'script5', 's3://bucket/script5');
