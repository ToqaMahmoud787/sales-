(() => {
    "use strict";
    const settings = JSON.parse(document.getElementById("deliverySettings").textContent);
    const errorBox = document.getElementById("clientError");
    const showError = message => {
        errorBox.textContent = message;
        errorBox.classList.remove("d-none");
    };
    if (!window.bootstrap || !window.DataTable || !window.Swal) {
        showError(settings.scriptsError);
        return;
    }
    document.querySelectorAll(".toast").forEach(el => bootstrap.Toast.getOrCreateInstance(el).show());
    const table = new DataTable("#dataTable", {
        order: [[0, "asc"]],
        pageLength: 10,
        layout: { topStart: null, topEnd: null, bottomStart: "info", bottomEnd: "paging" },
        language: settings.table,
        columnDefs: [{ targets: 5, orderable: false, searchable: false }]
    });
    document.getElementById("searchInput").addEventListener("input", event => {
        table.search(event.target.value).draw();
    });
    document.getElementById("addModal").addEventListener("show.bs.modal", event => {
        event.target.querySelector("form").reset();
    });
    document.getElementById("dataTable").addEventListener("click", async event => {
        const edit = event.target.closest(".edit-term");
        const remove = event.target.closest(".delete-term");
        if (edit) {
            edit.disabled = true;
            try {
                const url = new URL(settings.editUrl, window.location.origin);
                url.searchParams.set("code", edit.dataset.code);
                const response = await fetch(url, {
                    credentials: "same-origin", headers: { Accept: "application/json" }
                });
                if (response.status === 401 || response.redirected) {
                    window.location.assign(settings.loginUrl); return;
                }
                const result = await response.json();
                if (!response.ok || !result.status) {
                    showError(result.message || settings.apiError); return;
                }
                const modal = document.getElementById("editModal");
                modal.querySelectorAll("[data-field]").forEach(input => {
                    input.value = result.data[input.dataset.field] ?? "";
                });
                bootstrap.Modal.getOrCreateInstance(modal).show();
            } catch {
                showError(settings.apiError);
            } finally {
                edit.disabled = false;
            }
        }
        if (remove) {
            const result = await Swal.fire({
                title: settings.deleteTitle,
                text: settings.deleteText,
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: settings.confirm,
                cancelButtonText: settings.cancel,
                confirmButtonColor: "#b42318",
                focusCancel: true
            });
            if (result.isConfirmed) {
                document.getElementById("deleteCode").value = remove.dataset.code;
                document.getElementById("deleteForm").requestSubmit();
            }
        }
    });
    document.querySelectorAll("form").forEach(form => {
        form.addEventListener("submit", () => {
            form.querySelectorAll('button[type="submit"]').forEach(button => button.disabled = true);
        });
    });
})();
