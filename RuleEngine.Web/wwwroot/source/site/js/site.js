var apiUrl = '/Proxy/';
var editor;
var filterDefinition = [];
var jqueryQueryBuilder;

$("#builder").on("afterCreateRuleFilters.queryBuilder",
    function (e, rule) {
        initDropdown(".rule-filter-container", rule, "select2");
    });

$("#builder").on("afterCreateRuleOperators.queryBuilder",
    function (e, rule) {
        initDropdown(".rule-operator-container", rule, "select2");
    });

$("#builder").on("afterCreateRuleInput.queryBuilder",
    function (e, rule) {
        initDropdown(".rule-value-container", rule, "text");
    });

$("#fetchScheme").on("click",
    function () {
        var url = $("#jsonSchemeAddress").val();

        if (!isValidUrl(url)) {
            alert("Entered wrong url.");
            return;
        }

        var jsonData = JSON.stringify({ Url: url });
        $.ajax({
            url: "/Fetch/",
            data: jsonData,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                editor.set(response);
            },
            error: function () {
                alert("Invalid JSON data.");
            }
        });
    });

$("#parseJson").on("click",
    function () {
        $(".query-row").hide();
        filterDefinition = [];

        try {
            var json = JSON.stringify(editor.get());
            var data = JSON.parse(json);
            var validationErrors = validateJsonFields(data);

            if (validationErrors.length > 0) {
                alert(validationErrors.join("\n"));
                return;
            }

            $.each(data, function (i, n) {
                filterDefinition.push({
                    "label": n.label,
                    "field": n.field,
                    "type": n.type,
                    "input": null,
                    "multiple": null,
                    "values": null,
                    "operators": null,
                    "template": null,
                    "itemBankNotFilterable": null,
                    "itemBankNotColumn": null,
                    "id": i + 1
                });
            });

            if (filterDefinition.length > 0) {
                $(".query-row").show();
                loadQueryBuilder();
            }
            else
                $(".query-row").hide();

        } catch (e) {
            alert("Invalid JSON data");
        }
    });

$("#save-rules").on("click",
    function () {
        if (!jqueryQueryBuilder.queryBuilder("validate")) {
            alert("Your rules is incorrect.");
            return;
        }

        var data = jqueryQueryBuilder.queryBuilder("getRules");
        var json = {
            Name: $("#ruleName").val(),
            Filter: JSON.stringify(editor.get()),
            Content: JSON.stringify(data)
        };
        var jsonData = JSON.stringify(json);

        $.ajax({
            url: apiUrl + "/Rules",
            data: jsonData,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response) {
                    if (response.success)
                        alert("Rules saved successfully.");
                    else
                        alert("An error occurred while rules saving.");
                }
                else
                    alert("An error occurred while rules saving.");
            },
            error: function () {
                alert("Error!!!");
            }
        });
    });

var container = document.getElementById('jsoneditor');
var sampleJsonData = [
    {
        "label": "",
        "field": "",
        "type": ""
    }
];
var options = {
    mode: 'code',
    onError: function (err) {
        alert(err.toString());
    },
    onModeChange: function (newMode, oldMode) {
    }
};

editor = new JSONEditor(container, options, sampleJsonData);

function loadQueryBuilder() {
    jqueryQueryBuilder = $("#builder").queryBuilder({
        plugins: ["bt-tooltip-errors", "filter-description", "sortable"],
        allow_empty: false,
        filters: filterDefinition,
        conditions: ["AND", "OR", "NOT"],
        sortable: true,
        icons: {
            add_group: "fa fa-plus-square",
            add_rule: "fa fa-plus-circle",
            remove_group: "fa fa-minus-square",
            remove_rule: "fa fa-minus-circle",
            error: "fa fa-exclamation-triangle",
            sortable: "fa fa-exclamation-triangle"
        }
    });
}

function initDropdown(containerName, rule, type) {
    var select = rule.$el.find(containerName + " > select");
    if (select) {
        var values = [];

        $.each(select.find("option"),
            function () {
                var text = $(this).text();
                var value = $(this).val();

                values.push({
                    id: value,
                    text: text
                });
            });

        var initialValue = values.length > 0 ? values[0].id : "";
        var hideElement = containerName + " > select";

        if (type === "text") {
            initialValue = null;
            hideElement = containerName + " > input";
        }

        rule.$el.find(hideElement).hide();

        var element = $("<a />",
            {
                "href": "#",
                "data-type": type,
                "data-value": initialValue
            });

        rule.$el.find(containerName).append(element);

        element.editable({
            source: values,
            select2: {
                theme: "bootstrap",
                width: "100%",
                dropdownAutoWidth: true
            }
        }).on("shown",
            function (e, editable) {
                setTimeout(function () {
                    if (type === "select2") {
                        editable.input.$input.select2("open").on("change",
                            function (e, params) {

                                rule.$el.find(containerName + " > select").val(e.target.value);
                                rule.$el.find(containerName + " > select").trigger("change");
                                rule.$el.find(containerName).find(".editable-submit").trigger("click");
                            });
                    }
                },
                    0);
            })
            .on("save", function (e, editable) {
                if (type === "text")
                    rule.$el.find(containerName + " > input").val(editable.submitValue);
                rule.$el.find(containerName + " > input").trigger("change");
            });
    }
}

function convertArraysToCommaDelimited(obj) {
    if (obj != null) {
        if (obj.hasOwnProperty("value")) {
            if (Object.prototype.toString.call(obj.value) === "[object Array]") {
                obj.value = obj.value.join(", ");
            }
        }
        if (obj.hasOwnProperty("rules") && obj.rules != null) {
            for (var i = 0; i < obj.rules.length; i++) {
                convertArraysToCommaDelimited(obj.rules[i]);
            }
        }
    }
}

function isValidUrl(url) {
    var pattern = "^(?!mailto:)(?:(?:http|https|ftp)://)(?:\\S+(?::\\S*)?@)?(?:(?:(?:[1-9]\\d?|1\\d\\d|2[01]\\d|22[0-3])(?:\\.(?:1?\\d{1,2}|2[0-4]\\d|25[0-5])){2}(?:\\.(?:[0-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-4]))|(?:(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)(?:\\.(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)*(?:\\.(?:[a-z\\u00a1-\\uffff]{2,})))|localhost)(?::\\d{2,5})?(?:(/|\\?|#)[^\\s]*)?$";
    var regex = new RegExp(pattern);

    return url.match(regex);
}

function validateJsonFields(data) {
    var errors = [];
    if (data) {
        $.each(data, function (i, n) {
            var rowNumber = i + 1;
            if (!n.hasOwnProperty("label"))
                errors.push("Your JSON data not contains 'label' field. Row number: " + rowNumber);

            if (!n.hasOwnProperty("field"))
                errors.push("Your JSON data not contains 'field' field. Row number: " + rowNumber);

            if (!n.hasOwnProperty("type"))
                errors.push("Your JSON data not contains 'type' field. Row number: " + rowNumber);
        });
    }

    return errors;
}