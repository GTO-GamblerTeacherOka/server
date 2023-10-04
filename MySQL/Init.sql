CREATE DATABASE IF NOT EXISTS `virce`;
USE `virce`;

CREATE TABLE IF NOT EXISTS `RoomType` (
    `TypeID` TINYINT NOT NULL,
    `TypeDetail` VARCHAR(255) NOT NULL,
    PRIMARY KEY (`TypeID`)
);

CREATE TABLE IF NOT EXISTS `Room` (
    `RoomID` TINYINT NOT NULL AUTO_INCREMENT,
    `RoomType` TINYINT NOT NULL,
    FOREIGN KEY (`RoomType`) REFERENCES `RoomType`(`TypeID`),
    PRIMARY KEY (`RoomID`)
);

CREATE TABLE IF NOT EXISTS `User` (
    `UserID` TINYINT NOT NULL,
    `RoomID` TINYINT NOT NULL,
    FOREIGN KEY (`RoomID`) REFERENCES `Room`(`RoomID`),
    `ModelID` TEXT NOT NULL,
    `DisplayName` TEXT NOT NULL,
    `IPAddress` TEXT NOT NULL,
    `Port` SMALLINT UNSIGNED NOT NULL,
    PRIMARY KEY (`UserID`, `RoomID`)
);

INSERT IGNORE INTO `RoomType` (`TypeID`, `TypeDetail`) VALUES (0, 'Lobby');
INSERT IGNORE INTO `RoomType` (`TypeID`, `TypeDetail`) VALUES (1, 'Lace');
