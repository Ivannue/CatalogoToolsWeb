# CatalogoToolsWeb

Un catálogo web moderno y responsivo para explorar herramientas de desarrollo web.

## 🚀 Características

- **Catálogo de Herramientas**: Explora una colección curada de herramientas para desarrollo web
- **Búsqueda en Tiempo Real**: Encuentra herramientas rápidamente con el buscador integrado
- **Filtrado por Categorías**: Filtra herramientas por Frontend, Backend, Diseño o DevOps
- **Diseño Responsivo**: Se adapta perfectamente a cualquier dispositivo
- **Interfaz Moderna**: UI limpia y atractiva con animaciones suaves

## 📁 Estructura del Proyecto

```
CatalogoToolsWeb/
├── index.html          # Página principal
├── css/
│   └── styles.css      # Estilos de la aplicación
├── js/
│   ├── app.js          # Lógica principal de la aplicación
│   └── tools-data.js   # Datos de las herramientas
└── README.md           # Documentación
```

## 🛠️ Uso

1. Clona el repositorio:
   ```bash
   git clone https://github.com/Ivannue/CatalogoToolsWeb.git
   ```

2. Abre `index.html` en tu navegador web favorito

3. ¡Explora las herramientas!

## 🎨 Categorías Disponibles

- **Frontend**: Herramientas para desarrollo del lado del cliente
- **Backend**: Herramientas para desarrollo del lado del servidor
- **Diseño**: Herramientas para diseño UI/UX
- **DevOps**: Herramientas para integración y despliegue continuo

## 📝 Personalización

Para agregar nuevas herramientas, edita el archivo `js/tools-data.js` y añade objetos con la siguiente estructura:

```javascript
{
    id: 13,
    name: "Nombre de la Herramienta",
    description: "Descripción breve de la herramienta.",
    category: "frontend", // frontend, backend, design, devops
    icon: "🔧",
    url: "https://ejemplo.com/"
}
```

## 📄 Licencia

Este proyecto está disponible bajo la licencia MIT.
