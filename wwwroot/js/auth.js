function togglePasswordVisibility(inputId, button) {
    const input = document.getElementById(inputId);
    if (input.type === "password") {
        input.type = "text";
        button.innerHTML = `
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3l18 18" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.584 10.587a2 2 0 002.828 2.83" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.363 5.365A9.466 9.466 0 0112 5c4.478 0 8.268 2.943 9.542 7a9.957 9.957 0 01-1.84 3.18m-6.21 1.282A9.461 9.461 0 0112 19c-4.478 0-8.268-2.943-9.542-7a9.96 9.96 0 011.563-3.029" />
            </svg>
        `;
    } else {
        input.type = "password";
        button.innerHTML = `
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
            </svg>
        `;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const candidateForm = document.getElementById('candidateRegisterForm');
    if (candidateForm) {
        candidateForm.addEventListener('submit', function () {
            const fullNameInput = document.getElementById('fnID');
            if (fullNameInput) {
                fullNameInput.value = fullNameInput.value.trim();
            }
        });
    }
});
