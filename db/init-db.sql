CREATE DATABASE IF NOT EXISTS userdb;
CREATE DATABASE IF NOT EXISTS productdb;
CREATE DATABASE IF NOT EXISTS orderdb;
CREATE DATABASE IF NOT EXISTS eventstoredb;
GRANT ALL PRIVILEGES ON userdb.* TO 'my_user'@'%' IDENTIFIED BY 'my_password';
GRANT ALL PRIVILEGES ON productdb.* TO 'my_user'@'%' IDENTIFIED BY 'my_password';
GRANT ALL PRIVILEGES ON orderdb.* TO 'my_user'@'%' IDENTIFIED BY 'my_password';
GRANT ALL PRIVILEGES ON eventstoredb.* TO 'my_user'@'%' IDENTIFIED BY 'my_password';
FLUSH PRIVILEGES;
