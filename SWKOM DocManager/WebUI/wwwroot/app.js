//const apiUrl = 'http://host.docker.internal:8081/document';
const apiUrl = 'http://localhost:8081/document';

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
        .catch(error => console.error('Fehler beim Abrufen der Documents:', error));
}


document.addEventListener('DOMContentLoaded', function () {
    // Expand/collapse form logic remains the same
    const errorMessageDiv = document.getElementById('error-message');
    const successMessageDiv = document.getElementById('success-message');
    const expandBtn = document.getElementById('expand-btn');
    const form = document.getElementById('expandable-form');

    // Function to show a message for 5 seconds
    function showMessage(messageDiv, message) {
        messageDiv.style.display = 'flex'; // Show the message div
        messageDiv.querySelector('span').textContent = message; // Set the message content

        // Hide the message after 5 seconds
        setTimeout(() => {
            messageDiv.style.display = 'none'; // Hide the message div
        }, 5000);
    }

    // Hide messages initially
    errorMessageDiv.style.display = 'none';
    successMessageDiv.style.display = 'none';

    expandBtn.addEventListener('click', function () {
        const form = document.getElementById(this.dataset.target);
        const isOpen = form.classList.toggle('open'); // Toggle the 'open' class

        // Toggle the button text between "Add Document" and "Cancel"
        this.textContent = isOpen ? 'Cancel' : 'Add Document';

        // Change the button background color based on the text
        this.style.backgroundColor = isOpen ? '#c1121f' : '';

        this.style.color = isOpen ? '#ffffff' : '';

    });
  
        
    form.addEventListener('submit', function (event) {
        event.preventDefault(); // Prevent default form submission behavior

        const title = document.getElementById('title').value;
        const author = document.getElementById('author').value;

        // Create a document object to send
        const documentData = {
            title: title,
            author: author
        };

        // Send a POST request to the API
        fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(documentData)
        })
            .then(response => {
                if (response.ok) {
                    return response.json(); // Parse the JSON response
                } else {
                    throw new Error('Failed to submit document');
                }
            })
            .then(data => {
                console.log('Document submitted successfully:', data);
                fetchDocuments(); // Refresh the document list after successful submission

                // Show success message
                showMessage(successMessageDiv, 'Document submitted successfully!');
            })
            .catch(error => {
                console.error('Error submitting document:', error);

                // Show error message
                showMessage(errorMessageDiv, 'An unexpected error occurred while submitting the document.');
            });
    });
});

document.addEventListener('DOMContentLoaded', fetchDocuments);

