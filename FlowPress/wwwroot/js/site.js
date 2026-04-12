// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function openNewsModal(card) {
    const title = card.dataset.title;
    const description = card.dataset.description;
    const source = card.dataset.source;
    const date = card.dataset.date;
    const url = card.dataset.url;
    const image = card.dataset.image;

    document.getElementById('modal-title').textContent = title;
    document.getElementById('modal-description').textContent = description;
    document.getElementById('modal-source').textContent = source?.toUpperCase();
    document.getElementById('modal-date').textContent = date;
    document.getElementById('modal-link').href = url;

    const imgEl = document.getElementById('modal-image');
    if (image) {
        imgEl.src = image;
        imgEl.style.display = 'block';
    } else {
        imgEl.style.display = 'none';
    }

    const modal = new bootstrap.Modal(document.getElementById('newsModal'));
    modal.show();
}