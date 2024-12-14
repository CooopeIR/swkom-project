//const apiUrl = 'http://host.docker.internal:8081/document';
const apiUrl = 'http://localhost:8081/document';

// Function to fetch and display Todo items
function fetchDocuments() {
    console.log('Fetching documents...');
    //fetchUrl = apiUrl;
    //if (searchQuery) {
    //    fetchUrl = apiUrl + `/?search=${encodeURIComponent(searchQuery)}`;
    //}

    fetch(fetchUrl)
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
                    <button class="view" onclick="viewDocument(${documentFromResponse.id})">view</button>    
                    <button class="delete" onclick="deleteDocument(${documentFromResponse.id})">
                            <span class="material-icons">delete</span>
                    </button>
                </div>
                `;
                documentList.appendChild(li);
            });
        })
        .catch (error => {
            console.error('Fehler beim Abrufen der Documents:', error);
        });
}

function searchDocumenrts(questyString) {
    const exactMatch = document.getElementById("exactMatchToggle").checked;

    if (exactMatch) {
        fetchUrl = apiurl + ""
    }

}

//Listener functions after Site is loaded
document.addEventListener('DOMContentLoaded', function () {
    // Expand/collapse form logic remains the same
    const errorMessageDiv = document.getElementById('error-message');
    const successMessageDiv = document.getElementById('success-message');
    const expandBtn = document.getElementById('expand-btn');
    const form = document.getElementById('my-form');
    const searchBtn = document.getElementById('search-btn');
    const searchTerm = document.getElementById('search-term');
    const clearBtn = document.getElementById("clear-btn");

    //Open window for file upload - file selection
    document.getElementById("upload-btn").addEventListener("click", () => {
        document.getElementById("fileupload").click();
    });

    //Show specific message for successfully uploaded file
    document.getElementById("fileupload").addEventListener("change", (event) => {
        showMessage(successMessageDiv, "File uploaded successfully!");

        const fileNameElement = document.getElementById("file-name");
        const file = event.target.files[0]; // Get the selected file

        // Display the file name or a placeholder if no file is selected
        fileNameElement.textContent = file ? `${file.name}` : "No file selected";
    });


    // Function to show a message for 5 seconds
    function showMessage(messageDiv, message) {
        messageDiv.style.display = 'flex'; // Show the message div
        messageDiv.querySelector('span').textContent = message; // Set the message content

        // Hide the message after 5 seconds
        setTimeout(() => {
            messageDiv.style.display = 'none'; // Hide the message div
        }, 5000);
    }

    //Search Button; Get search term from search term input field and send filter request with searchTerm
    searchBtn.addEventListener('click', function () {
        let query = searchTerm.value;

        if (query.length === 0)
            showMessage(errorMessageDiv, "Search Term cannot be empty!");
        else {
            fetchDocuments(query);
            clearBtn.style.display = "block";
            setTimeout(() => {
                clearBtn.classList.add("show"); // Add the class to trigger sliding effect
            }, 40);
        }
    });

    //Clear Button; Clear search term input field and refresh shown / listed documents
    clearBtn.addEventListener("click", () => {
        // Clear the input field
        searchTerm.value = "";
        fetchDocuments('');
        // Hide the clear button with sliding effect
        clearBtn.classList.remove("show");
        setTimeout(() => {
            clearBtn.style.display = "none"; // Hide after sliding closed
        }, 300); // Match this duration with CSS transition time
    });

    // Hide messages initially
    errorMessageDiv.style.display = 'none';
    successMessageDiv.style.display = 'none';

    //Expand Button; open and closes formular with input fields (for document addition)
    expandBtn.addEventListener('click', function () {
        const form = document.getElementById(this.dataset.target);
        const isOpen = form.classList.toggle('open'); // Toggle the 'open' class

        // Toggle the button text between "Add Document" and "Cancel"
        this.textContent = isOpen ? 'Cancel' : 'Add Document';

        // Change the button background color based on the text
        this.style.backgroundColor = isOpen ? `#FF4D4D` : '';

        this.style.color = isOpen ? '#ffffff' : '';
    });

    //Submit Button; get Data from input fields and post these to REST container
    form.addEventListener('submit', function (event) {
        event.preventDefault(); // Prevent default form submission behavior

        // Select form input values
        const title = document.getElementById("title").value;
        const author = document.getElementById("author").value;
        const fileInput = document.getElementById("fileupload");

        // Create a new FormData object
        const formData = new FormData();
        formData.append("title", title);               // Add title
        formData.append("author", author);             // Add author
        formData.append("uploadedfile", fileInput.files[0]);  // Add the file


        // Log whether each form field is present in formData
        console.log("Title:", formData.has("title") ? formData.get("title") : "Not provided");
        console.log("Author:", formData.has("author") ? formData.get("author") : "Not provided");
        console.log("File:", formData.has("uploadedfile") ? formData.get("uploadedfile").name : "No file uploaded");

        const fetchOptions = {
            method: 'POST',
            body: formData
        }

        // Send a POST request to the API
        fetch(apiUrl, fetchOptions)
            .then(response => {
                if (response.ok) {
                    document.getElementById("file-name").textContent = "";
                    form.reset();
                    return response.json(); // Parse the JSON response
                } else {
                    response.json().then(err => {
                        console.log(Object.values(err.errors));
                        Object.values(err.errors).forEach((singleError) => showMessage(errorMessageDiv, singleError));
                    });
                    throw new Error('');
                }
            })
            .then(data => {
                console.log('Document submitted successfully:', data);
                fetchDocuments(); // Refresh the document list after successful submission
                showMessage(successMessageDiv, 'Document submitted successfully!');
            })
            .catch(error => {
                console.error('Submission error:', error);
                showMessage(errorMessageDiv, error.message || 'An unknown error occurred.');
            });
    });
});

document.addEventListener('DOMContentLoaded', fetchDocuments(''));

//Delete function to delete a single document with ID of clicked document button
function deleteDocument(id) {
    fetch(`${apiUrl}/${id}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                fetchDocuments(); // Refresh the list after deletion
            } else {
                console.error('Fehler beim Löschen des Dokuments.');
            }
        })
        .catch(error => console.error('Fehler:', error));
}

function viewDocument(id) {
    fetch(`${apiUrl}/${id}`, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                fetchDocuments(); // Refresh the list after deletion
                return response.json(); // Return the parsed JSON
            } else {
                console.error('Fehler beim Holen der Dokument Daten.');
            }
        })
        .then(data => {
            console.log(data);
        })
        .catch(error => console.error('Fehler:', error));
}