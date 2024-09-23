const apiUrl = 'http://localhost:8081/document';

// Function to fetch and display Todo items
function fetchDocuments() {
    console.log('Fetching Todo items...');
    fetch(apiUrl)
        .then(response => response.json())
        .then(data => {
            const documentList = document.getElementById('document-list');
            documentList.innerHTML = ''; // Clear the list before appending new items
            data.forEach(document => {
                // Create list item with delete and toggle complete buttons
                const li = document.createElement('div');
                li.innerHTML = `
                    <div class="container">
                    <span>Task: ${document.name}</span>
                    <button class="delete" style="margin-left: 10px;" onclick="deleteTask(${document.id})">Delete</button>
                    <button style="margin-left: 10px;" onclick="toggleComplete(${document.id}, '${document.name}')">
                        Mark as ${document.isComplete ? 'Incomplete' : 'Complete'}
                    </button>
                    </div>
                `;
                documentList.appendChild(li);
            });
        })
        .catch(error => console.error('Fehler beim Abrufen der Todo-Items:', error));
}


// Function to add a new task
function addTask() {
    const taskName = document.getElementById('taskName').value;
    const isComplete = document.getElementById('isComplete').checked;

    if (taskName.trim() === '') {
        alert('Please enter a task name');
        return;
    }

    const newTask = {
        name: taskName,
        isComplete: isComplete
    };

    fetch(apiUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newTask)
    })
        .then(response => {
            if (response.ok) {
                fetchDocuments(); // Refresh the list after adding
                document.getElementById('taskName').value = ''; // Clear the input field
                document.getElementById('isComplete').checked = false; // Reset checkbox
            } else {
                // Neues Handling für den Fall eines Fehlers (z.B. leeres Namensfeld)
                response.json().then(err => alert("Fehler: " + err.message));
                console.error('Fehler beim Hinzufügen der Aufgabe.');
            }
        })
        .catch(error => console.error('Fehler:', error));
}


// Function to delete a task
function deleteTask(id) {
    fetch(`${apiUrl}/${id}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                fetchDocuments(); // Refresh the list after deletion
            } else {
                console.error('Fehler beim Löschen der Aufgabe.');
            }
        })
        .catch(error => console.error('Fehler:', error));
}

// Function to toggle complete status
function toggleComplete(id, isComplete, name) {
    // Aufgabe mit umgekehrtem isComplete-Status aktualisieren
    const updatedTask = {
        id: id,  // Die ID des Tasks
        name: name, // Der Name des Tasks
        isComplete: !isComplete // Status umkehren
    };

    fetch(`${apiUrl}/${id}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(updatedTask)
    })
        .then(response => {
            if (response.ok) {
                fetchDocuments(); // Liste nach dem Update aktualisieren
                console.log('Erfolgreich aktualisiert.');
            } else {
                console.error('Fehler beim Aktualisieren der Aufgabe.');
            }
        })
        .catch(error => console.error('Fehler:', error));
}

// Load todo items on page load
document.addEventListener('DOMContentLoaded', fetchDocuments);