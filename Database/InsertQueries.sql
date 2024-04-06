-- Insert data into scripttypelookup
INSERT INTO scripttypelookup (type) VALUES ('Bash'), ('Python');

-- Insert data into developers
INSERT INTO developers (username) VALUES ('developer1'), ('developer2'), ('developer3');

-- Insert data into scripts
INSERT INTO scripts (dev_id, script_name, script_s3_uri, script_type) VALUES 
    (1, 'script1', 's3://bucket/script1', 1),
    (2, 'script2', 's3://bucket/script2', 2),
    (3, 'script3', 's3://bucket/script3', 1),
    (1, 'script4', 's3://bucket/script4', 2),
    (2, 'script5', 's3://bucket/script5', 1);
