CREATE DATABASE  IF NOT EXISTS `eduquiz` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `eduquiz`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: eduquiz
-- ------------------------------------------------------
-- Server version	5.5.5-10.1.30-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `categoria`
--

DROP TABLE IF EXISTS `categoria`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `categoria` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `imagen` varchar(255) DEFAULT NULL,
  `external_id` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categoria`
--

LOCK TABLES `categoria` WRITE;
/*!40000 ALTER TABLE `categoria` DISABLE KEYS */;
INSERT INTO `categoria` VALUES (2,'Ciencia',NULL,'e4d105e8-af00-40f6-90b8-022df6eb2bfb'),(3,'Geofrafia',NULL,'e2d135e8-ab10-40d6-90a8-086df6cb7bfx'),(4,'Historia',NULL,'a2c13568-ab10-40d6-90a8-0865f6eb7cfa'),(5,'Musica',NULL,'e2d78578-a560-4456-12a8-0889f67b734x'),(6,'Deportes',NULL,'e2d78578-a561-4456-12a8-0889f67b734x'),(7,'Matematicas',NULL,'e2d78578-a560-4456-12a8-0889a67b734x'),(8,'Astronomia',NULL,'b2d78578-a560-4456-12a8-0889f67b734x');
/*!40000 ALTER TABLE `categoria` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nivel_usuario`
--

DROP TABLE IF EXISTS `nivel_usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `nivel_usuario` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nivel` int(3) NOT NULL DEFAULT '1',
  `experiencia` int(11) NOT NULL DEFAULT '0',
  `clasificacion` int(11) NOT NULL DEFAULT '1',
  `id_persona` int(11) NOT NULL,
  `puntaje` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `id_persona` (`id_persona`),
  CONSTRAINT `nivel_usuario_ibfk_1` FOREIGN KEY (`id_persona`) REFERENCES `persona` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nivel_usuario`
--

LOCK TABLES `nivel_usuario` WRITE;
/*!40000 ALTER TABLE `nivel_usuario` DISABLE KEYS */;
INSERT INTO `nivel_usuario` VALUES (1,4,400,1,2,105),(2,1,0,0,4,0),(3,1,0,0,5,0),(4,1,0,0,6,0),(5,1,0,0,1,0),(6,1,0,0,3,0),(9,1,0,0,11,0),(10,1,0,0,12,0);
/*!40000 ALTER TABLE `nivel_usuario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `opcion`
--

DROP TABLE IF EXISTS `opcion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `opcion` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `opcion` varchar(255) NOT NULL,
  `estado` varchar(20) NOT NULL,
  `id_pregunta` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_pregunta` (`id_pregunta`),
  CONSTRAINT `opcion_ibfk_1` FOREIGN KEY (`id_pregunta`) REFERENCES `pregunta` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `opcion`
--

LOCK TABLES `opcion` WRITE;
/*!40000 ALTER TABLE `opcion` DISABLE KEYS */;
INSERT INTO `opcion` VALUES (1,'Rusia','Correcta',1),(2,'Alemania','Incorrecta',1),(3,'Brasil','Incorrecta',1),(4,'Africa','Incorrecta',1),(5,'100 ºC','Correcta',2),(6,'30 ºC','Incorrecta',2),(7,'74 ºC','Incorrecta',2),(8,'45 ºC','Incorrecta',2),(9,'1492','Correcta',3),(10,'1501','Incorrecta',3),(11,'1509','Incorrecta',3),(12,'1489','Incorrecta',3),(13,'Si','Correcta',4),(14,'No','Incorrecta',4),(15,'Depende del clima','Incorrecta',4),(16,'Depende del tipo de planta','Incorrecta',4);
/*!40000 ALTER TABLE `opcion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `persona`
--

DROP TABLE IF EXISTS `persona`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `persona` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(60) NOT NULL,
  `correo` varchar(80) NOT NULL,
  `clave` varchar(30) NOT NULL,
  `rol` tinyint(4) NOT NULL DEFAULT '1',
  `estado` tinyint(4) NOT NULL DEFAULT '1',
  `external_id` varchar(100) NOT NULL,
  `imagen` varchar(255) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`id`),
  UNIQUE KEY `correo_UNIQUE` (`correo`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `persona`
--

LOCK TABLES `persona` WRITE;
/*!40000 ALTER TABLE `persona` DISABLE KEYS */;
INSERT INTO `persona` VALUES (1,'Erick Jara V','erickuchiha3@gmail.com','Wargosh',0,1,'2e3c79c1-9858-4590-a8a3-d2d8f7b6063b',NULL,'2018-06-23 08:04:04','2018-06-23 20:01:04'),(2,'Sami Gonzalez','samisamantha@gmail.com','6789',1,1,'f68f01cf-9ffb-4660-a75d-43d3d0877a2e',NULL,'2018-06-23 08:07:57','2018-06-23 08:07:57'),(3,'Gerardo Ramirez','gedoram@gmail.com','55555',0,1,'10da295c-f0f2-423b-8e45-df7fc26bb09d',NULL,'2018-06-23 09:06:28','2018-06-23 09:06:28'),(4,'Ricardo Esparza','riky@gmail.com','4444',0,1,'43707048-432a-4ce7-9f30-2fed024ea828',NULL,'2018-06-24 16:39:18','2018-06-24 16:39:18'),(5,'Jena','jeandar@gmail.com','12345',1,1,'6ba2e0fe-115d-4f51-8414-7d3e74b00371',NULL,'2018-06-25 03:14:49','2018-06-25 03:14:49'),(6,'Asasas','er@gmail.com','asd',1,1,'1798fa5d-640c-4b4a-ba8d-e2b88e488cfc',NULL,'2018-06-27 15:17:11','2018-06-27 15:17:11'),(9,'Liana Ochoa','lian@gmail.com','asdf',1,1,'44d02fef-aff7-4375-bdab-cf6b329e6b86',NULL,'2018-07-03 02:12:59','2018-07-03 02:12:59'),(11,'Holi Boli','holiboli@gmial.com','12345',1,1,'2cfeea8f-ced9-428c-86f4-df483abcf0a1',NULL,'2018-07-03 02:31:56','2018-07-03 02:31:56'),(12,'Julio Ca?ar','lex@gmail.com','lex',1,1,'cb9b3f3f-f758-4059-9308-298c684153ef',NULL,'2018-07-03 02:56:35','2018-07-03 02:56:35');
/*!40000 ALTER TABLE `persona` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `preg_cate`
--

DROP TABLE IF EXISTS `preg_cate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `preg_cate` (
  `id_pregunta` int(11) NOT NULL,
  `id_categoria` int(11) NOT NULL,
  KEY `id_pregunta` (`id_pregunta`),
  KEY `id_categoria` (`id_categoria`),
  CONSTRAINT `preg_cate_ibfk_1` FOREIGN KEY (`id_pregunta`) REFERENCES `pregunta` (`id`),
  CONSTRAINT `preg_cate_ibfk_2` FOREIGN KEY (`id_categoria`) REFERENCES `categoria` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `preg_cate`
--

LOCK TABLES `preg_cate` WRITE;
/*!40000 ALTER TABLE `preg_cate` DISABLE KEYS */;
INSERT INTO `preg_cate` VALUES (1,3),(2,2),(3,4),(4,5),(4,2);
/*!40000 ALTER TABLE `preg_cate` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pregunta`
--

DROP TABLE IF EXISTS `pregunta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pregunta` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `pregunta` varchar(255) NOT NULL,
  `dificultad` varchar(30) NOT NULL,
  `estado` tinyint(1) DEFAULT '1',
  `external_id` varchar(100) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `id_persona` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_persona` (`id_persona`),
  CONSTRAINT `pregunta_ibfk_1` FOREIGN KEY (`id_persona`) REFERENCES `persona` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pregunta`
--

LOCK TABLES `pregunta` WRITE;
/*!40000 ALTER TABLE `pregunta` DISABLE KEYS */;
INSERT INTO `pregunta` VALUES (1,'¿donde se realizó el mundial 2018?','Facil',1,'1798fa5d-640c-434a-cd8d-e2b88e48afc','2018-06-30 02:31:03','2018-06-30 02:31:03',1),(2,'¿A los cuántos grados centígrados hierve el agua?','Facil',1,'37888a9d-445a-63a7-cd2a-e4b4a6488ac','2018-06-30 02:31:59','2018-06-30 02:31:59',1),(3,'¿En que año Colón descubrió America?','Facil',1,'3c6988a5d-340c-4aca-fa8d-e2458ad4fffc','2018-07-01 02:31:59','2018-07-01 02:31:59',3),(4,'¿Es cierto que las flores crecen más rápido si suena música a su alrededor?','Facil',1,'1798fa5d-648c-4436-cd89-e2688ef8acc','2018-07-01 18:33:59','2018-07-01 18:33:59',4);
/*!40000 ALTER TABLE `pregunta` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'eduquiz'
--

--
-- Dumping routines for database 'eduquiz'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-07-07 21:49:47
