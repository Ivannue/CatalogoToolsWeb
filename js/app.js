/**
 * Main application logic for the Tools Catalog
 */

document.addEventListener('DOMContentLoaded', function() {
    const toolsGrid = document.getElementById('toolsGrid');
    const searchInput = document.getElementById('searchInput');
    const categoryButtons = document.querySelectorAll('.category-btn');
    
    let currentCategory = 'all';

    /**
     * Sanitize text for HTML output to prevent XSS
     * @param {string} text - The text to sanitize
     * @returns {string} - Sanitized text
     */
    function sanitizeHTML(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    /**
     * Render tools to the grid
     * @param {Array} tools - Array of tool objects to render
     */
    function renderTools(tools) {
        if (tools.length === 0) {
            toolsGrid.innerHTML = '<p class="no-results">No se encontraron herramientas que coincidan con tu búsqueda.</p>';
            return;
        }

        toolsGrid.innerHTML = tools.map(function(tool) {
            return '<article class="tool-card">' +
                '<span class="icon">' + sanitizeHTML(tool.icon) + '</span>' +
                '<h3>' + sanitizeHTML(tool.name) + '</h3>' +
                '<span class="category-tag">' + sanitizeHTML(getCategoryName(tool.category)) + '</span>' +
                '<p>' + sanitizeHTML(tool.description) + '</p>' +
                '<a href="' + sanitizeHTML(tool.url) + '" target="_blank" rel="noopener noreferrer" class="tool-link">Visitar</a>' +
            '</article>';
        }).join('');
    }

    /**
     * Get display name for category
     * @param {string} category - Category key
     * @returns {string} - Display name
     */
    function getCategoryName(category) {
        var names = {
            'frontend': 'Frontend',
            'backend': 'Backend',
            'design': 'Diseño',
            'devops': 'DevOps'
        };
        return names[category] || category;
    }

    /**
     * Filter tools based on search term and category
     */
    function filterTools() {
        var searchTerm = searchInput.value.toLowerCase().trim();
        
        var filteredTools = toolsData.filter(function(tool) {
            var matchesSearch = tool.name.toLowerCase().includes(searchTerm) ||
                               tool.description.toLowerCase().includes(searchTerm);
            var matchesCategory = currentCategory === 'all' || tool.category === currentCategory;
            
            return matchesSearch && matchesCategory;
        });

        renderTools(filteredTools);
    }

    // Set up search input handler
    searchInput.addEventListener('input', filterTools);

    // Set up category button handlers
    categoryButtons.forEach(function(button) {
        button.addEventListener('click', function() {
            // Update active button
            categoryButtons.forEach(function(btn) {
                btn.classList.remove('active');
            });
            button.classList.add('active');
            
            // Update current category and filter
            currentCategory = button.dataset.category;
            filterTools();
        });
    });

    // Initial render
    renderTools(toolsData);
});
