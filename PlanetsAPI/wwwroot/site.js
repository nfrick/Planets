const uri = "api/planetm";
let planets = null;

// Contagem de planetas cadastrados
function getCount(count) {
    $("#counter").text(count + " planeta" + (count === 1 ? "" : "s"));
}

$(document).ready(function () {
    getData();
});

// Lê os planetas do BD e exibe na tabela
function getData() {
    $.ajax({
        type: "GET",
        url: uri,
        cache: false,
        success: function (data) {
            const tBody = $("#planets");

            $(tBody).empty();

            getCount(data.length);
            data.sort(function (a, b) {
                if (a.name < b.name) return -1;
                if (a.name > b.name) return 1;
                return 0;
            });
            $.each(data, function (key, item) {
                const tr = $("<tr></tr>")
                    .append($("<td></td>").text(item.name))
                    .append($("<td></td>").text(item.climate))
                    .append($("<td></td>").text(item.terrain))
                    .append($("<td class='number'></td>").text(item.movies))
                    .append($("<td class='number'></td>").text(item.swapiId))
                    .append($("<td></td>").text(item.swapiURL))
                    .append(
                        $("<td></td>").append(
                            $("<button>Editar</button>").on("click", function () {
                                editItem(item.id);
                            })
                        )
                    )
                    .append(
                        $("<td></td>").append(
                        $("<button class='button-delete'>Deletar</button>").on("click", function () {
                                deleteItem(item.id);
                            })
                        )
                    );

                tr.appendTo(tBody);
            });

            planets = data;
        }
    });
}


// Adicionar planeta ao banco de dados
function addItem() {
    const item = {
        name: $("#add-name").val(),
        climate: $("#add-climate").val(),
        terrain: $("#add-terrain").val()
    };

    $.ajax({
        type: "POST",
        accepts: "application/json",
        url: uri,
        contentType: "application/json",
        data: JSON.stringify(item),
        error: function (jqXHR, textStatus, errorThrown) {
            if ($("#add-name").val() === "")
                alert("Nome em branco!");
            else
                alert("Planeta " + item.name + " já existente ou não Star Wars!");
        },
        success: function (result) {
            getData();
            $("#add-name").val("");
            $("#add-climate").val("");
            $("#add-terrain").val("");
        }
    });
}

// Deletar planeta do banco de dados
function deleteItem(id) {
    $.ajax({
        url: uri + "/" + id,
        type: "DELETE",
        success: function (result) {
            getData();
        }
    });
}

// Atualizar planeta no banco de dados
function editItem(id) {
    $.each(planets, function (key, item) {
        if (item.id === id) {
            $("#edit-name").val(item.name);
            $("#edit-id").val(item.id);
            $("#edit-climate").val(item.climate);
            $("#edit-terrain").val(item.terrain);
        }
    });
    $("#editPlanet").css({ display: "block" });
    $("#addPlanet").css({ display: "none" });
}


// On submit
$(".editForm").on("submit", function () {
    const item = {
        name: $("#edit-name").val(),
        climate: $("#edit-climate").val(),
        terrain: $("#edit-terrain").val(),
        id: $("#edit-id").val()
    };

    $.ajax({
        url: uri + "/" + $("#edit-id").val(),
        type: "PUT",
        accepts: "application/json",
        contentType: "application/json",
        data: JSON.stringify(item),
        success: function (result) {
            getData();
        }
    });

    closeInput();
    return false;
});


// Depois de atualizar
function closeInput() {
    $("#editPlanet").css({ display: "none" });
    $("#addPlanet").css({ display: "block" });
}