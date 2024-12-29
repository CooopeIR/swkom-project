const baseUrl = 'http://localhost:8081/';
const documentUrl = baseUrl + 'document/view/';
const fileUrl = baseUrl + 'document/file/';

// Fetch document ID from query parameters
const urlParams = new URLSearchParams(window.location.search);
const documentId = urlParams.get('id');

//// Fetch and display document details
function fetchDocumentDetails() {
    let fetchUrl = new URL(documentId, documentUrl);
    

    // Hide content
    document.getElementById('expandable-meta-form').style.display = 'none';
    document.getElementById('expandable-ocr-form').style.display = 'none';

    // Show the loading spinner
    document.getElementById('details-loading-spinner').style.display = 'block';
    document.getElementById('ocr-loading-spinner').style.display = 'block';

    fetch(fetchUrl, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                console.error('Fehler beim Laden der Dokument Daten.');
            }
        })
        .then(data => {
            // Fill in the form values
            document.getElementById('id').value = data.id || '';  // Use `value` to fill form fields
            document.getElementById('title').value = data.title || '';
            document.getElementById('author').value = data.author || '';
            document.getElementById('filename').value = data.documentContentDto?.fileName || '';  // Ensure null-safe access
            document.getElementById('date-uploaded').value = data.documentMetadataDto?.uploadDate?.split('T')[0] || ''; // Format date to 'YYYY-MM-DD'
            document.getElementById('content-type').value = data.documentContentDto?.contentType || '';
            document.getElementById('file-size').value = data.documentMetadataDto?.fileSize || '';

            // Optionally, fill other fields as needed
            document.getElementById('ocr-text').innerHTML = data.ocrText || ''; // Assuming you have an input or textarea for OCR Text

            // Set the form to readonly
            const formElements = document.querySelectorAll('.my-form input');
            formElements.forEach(element => {
                element.readOnly = true; // Set each input to readonly
            });
            // Hide the loading spinner once the data is fetched and form is populated
            document.getElementById('details-loading-spinner').style.display = 'none';
            document.getElementById('ocr-loading-spinner').style.display = 'none';

            // Show content
            document.getElementById('expandable-meta-form').style.display = 'block';
            document.getElementById('expandable-ocr-form').style.display = 'block';
        })
        .catch(error => {
            console.error('Fehler:', error);
            // Handle errors and hide the spinner
            document.getElementById('details-loading-spinner').style.display = 'none';
            document.getElementById('ocr-loading-spinner').style.display = 'none';

            document.getElementById('expandable-meta-form').style.display = 'block';
            document.getElementById('expandable-ocr-form').style.display = 'block';
        });
}


// Navigate back to the main documents page
function goBack() {
    window.location.href = "index.html";
}

function fetchDocument() {
    const spinner = document.getElementById('doc-loading-spinner');
    const documentView = document.getElementById('document-view');
    const pdfViewer = document.getElementById('pdf-viewer');

    spinner.style.display = 'block';
    documentView.style.display = 'none';

    let fetchUrl = new URL(documentId, fileUrl);

    fetch(fetchUrl, {
        headers: {
            'Accept': '*/*',  // Accept any content type
            'Cache-Control': 'no-cache'  // Prevent caching if needed
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.blob();
        })
        .then(blob => {
            const pdfUrl = URL.createObjectURL(blob);
            pdfViewer.src = pdfUrl;

            // Clean up the object URL when the viewer is done
            pdfViewer.onload = () => {
                spinner.style.display = 'none';
                documentView.style.display = 'block';
            };
        })
        .catch(error => {
            console.error('Error fetching document:', error);
            spinner.style.display = 'none';
            // Show error message to user
            const errorMessage = document.getElementById('error-message');
            if (errorMessage) {
                errorMessage.textContent = 'Failed to load document. Please try again.';
                errorMessage.style.display = 'block';
            }
        });
}

// Clean up object URLs when navigating away
window.addEventListener('unload', () => {
    const pdfViewer = document.getElementById('pdf-viewer');
    if (pdfViewer && pdfViewer.src) {
        URL.revokeObjectURL(pdfViewer.src);
    }
});

document.addEventListener('DOMContentLoaded', function () {
    // Expand/collapse form logic remains the same
    const errorMessageDiv = document.getElementById('error-message');
    const successMessageDiv = document.getElementById('success-message');
    const form = document.getElementById('my-form');
    const backBtn = document.getElementById('back-btn');

    // Hide messages initially
    errorMessageDiv.style.display = 'none';
    successMessageDiv.style.display = 'none';

    backBtn.addEventListener('click', () => goBack());

    // Attach event listener to a specific button by ID
    const buttons = ['expand-ocr-btn', 'expand-meta-btn'];
    buttons.forEach(buttonId => {
        const button = document.getElementById(buttonId);
        if (button) {
            button.addEventListener('click', function () {
                const form = document.getElementById(this.dataset.target);
                const isOpen = form.classList.toggle('open'); // Toggle the 'open' class

                if (buttonId == 'expand-ocr-btn')
                    this.textContent = isOpen ? 'Hide' : 'Show Ocr Text';
                if (buttonId == 'expand-meta-btn')
                    this.textContent = isOpen ? 'Hide' : 'Show Meta Data';

                // Change the button background color based on the text
                this.style.backgroundColor = isOpen ? `#FF4D4D` : '';

                this.style.color = isOpen ? '#ffffff' : '';
            });
        }
    });
});

// Load the document details on page load
document.addEventListener("DOMContentLoaded", () => {
    fetchDocumentDetails();
    fetchDocument();
});
