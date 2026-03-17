# 📚 API de Biblioteca

## 🧩 Descripción del proyecto

Este proyecto consiste en el desarrollo de una API REST para la gestión de una biblioteca digital. Su propósito es ofrecer un sistema seguro, escalable y bien estructurado que permita administrar usuarios, autenticación y recursos relacionados con una biblioteca.

La API está diseñada siguiendo una arquitectura por capas, facilitando la separación de responsabilidades, el mantenimiento y la escalabilidad del sistema.

---

## 🎯 Objetivo

El objetivo principal es construir un backend que permita:

- Registrar y autenticar usuarios
- Gestionar roles y permisos
- Proteger endpoints mediante autenticación segura
- Administrar recursos de la biblioteca
- Implementar un sistema seguro de manejo de sesiones con JWT y Refresh Tokens

---

## 🔐 Seguridad

La API implementa un sistema de autenticación basado en:

- JWT (JSON Web Tokens) para acceso a recursos protegidos
- Refresh Tokens almacenados en base de datos
- Cookies HttpOnly para mitigar ataques XSS
- Configuración SameSite para prevenir CSRF
- Hash de tokens mediante HMACSHA256
- Rotación de Refresh Tokens

---

## 🏗️ Arquitectura

El proyecto sigue una arquitectura por capas:

- **Presentation (Controllers)** → Manejo de endpoints HTTP
- **Application (Services)** → Lógica de negocio
- **Domain (Entities)** → Modelos del sistema
- **Infrastructure** → Acceso a datos y configuración

---

## 👥 Gestión de usuarios

Se utiliza ASP.NET Identity para:

- Registro de usuarios
- Autenticación
- Manejo de roles (Admin, Usuario, etc.)
- Asignación de permisos

---

## 🔄 Flujo de autenticación

1. El usuario inicia sesión con sus credenciales
2. Se genera un Access Token (JWT)
3. Se genera un Refresh Token almacenado en base de datos
4. Los tokens se envían mediante cookies seguras
5. Cuando el Access Token expira, se usa el Refresh Token para obtener uno nuevo

---

## ⚙️ Tecnologías utilizadas

- ASP.NET Core
- Entity Framework Core
- ASP.NET Identity
- JWT
- SQL Server

---

## 🚀 Estado del proyecto

El proyecto se encuentra en desarrollo, enfocado en la implementación de autenticación segura, gestión de usuarios y estructura base del sistema.

---