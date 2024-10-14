const apiUrl = 'http://localhost:8081/documents';



// Function to fetch and display Todo items
function fetchDocuments() {
    console.log('Fetching all documents...');
    fetch(apiUrl)
        .then(response => {
            console.log(response); // Log the response
            return response.json(); // Return the parsed JSON
        })
        .then(data => {
            const documentList = document.getElementById('document-list');
            documentList.innerHTML = ''; // Clear the list before appending new items
            data.forEach(documentFromResponse => {
                // Create list item with delete and toggle complete buttons
                const li = document.createElement('div');
                li.classList.add('document-item');
                li.innerHTML = `
                <span class="document-name">${documentFromResponse.title} from ${documentFromResponse.author}</span>
                <div class="button-group">
                    <button class="view">view</button>    
                    <button class="delete">
                            <span class="material-icons">delete</span>
                    </button>
                </div>
                `;
                documentList.appendChild(li);
            });
        })
        .catch(error => console.error('Fehler beim Abrufen der Todo-Items:', error));
}

document.addEventListener('DOMContentLoaded', fetchDocuments);
document.addEventListener('DOMContentLoaded', function () {
    // Code to execute once the DOM is fully loaded
    const expandBtn = document.getElementById('expand-btn');

    expandBtn.addEventListener('click', function () {
        const form = document.getElementById(this.dataset.target);
        const isOpen = form.classList.toggle('open'); // Toggle the 'open' class

        // Toggle the button text between "Add Document" and "Cancel"
        this.textContent = isOpen ? 'Cancel' : 'Add Document';

        // Change the button background color based on the text
        this.style.backgroundColor = isOpen ? '#c1121f' : '';

        this.style.color = isOpen ? '#ffffff' : '';

    });
});

