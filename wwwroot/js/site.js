// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// date validation functions
function validateDates() {
    let isValid = true;
    
    // loop through all policy rows
    $('.policy-row').each(function (index) {
        const effectiveDateInput = $(this).find('.effective-date');
        const expirationDateInput = $(this).find('.expiration-date');
        const currentDateInput = $(this).find('.current-date');
        
        if (effectiveDateInput.val() && expirationDateInput.val() && currentDateInput.val()) {
            const effectiveDate = new Date(effectiveDateInput.val());
            const expirationDate = new Date(expirationDateInput.val());
            const currentDate = new Date(currentDateInput.val());
            
            // effective date must be before expiration date
            if (effectiveDate >= expirationDate) {
                showError(effectiveDateInput, 'Effective date must be before expiration date');
                showError(expirationDateInput, 'Expiration date must be after effective date');
                isValid = false;
            } else {
                clearError(effectiveDateInput);
                clearError(expirationDateInput);
            }
            
            // current date must be between effective and expiration dates
            if (currentDate < effectiveDate || currentDate > expirationDate) {
                showError(currentDateInput, 'Current date must be between effective and expiration dates');
                isValid = false;
            } else {
                clearError(currentDateInput);
            }
        }
    });
    
    return isValid;
}

function showError(input, message) {
    const errorElement = input.siblings('.field-validation-error');
    if (errorElement.length) {
        errorElement.text(message);
    } else {
        input.after(`<span class="field-validation-error text-danger">${message}</span>`);
    }
    input.addClass('input-validation-error');
}

function clearError(input) {
    input.siblings('.field-validation-error').remove();
    input.removeClass('input-validation-error');
}

// Set default dates if not already set
function setDefaultDates() {
    const today = new Date();
    const todayStr = today.toISOString().split('T')[0];
    
    // Default current date to today
    $('input[name$=".CurrentDate"]').each(function() {
        if (!$(this).val()) {
            $(this).val(todayStr);
        }
    });
    
    // Default effective date to first day of current month
    const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
    const firstDayStr = firstDayOfMonth.toISOString().split('T')[0];
    
    $('input[name$=".EffectiveDate"]').each(function() {
        if (!$(this).val()) {
            $(this).val(firstDayStr);
        }
    });
    
    // Default expiration date to one year from effective date
    $('input[name$=".ExpirationDate"]').each(function(index) {
        if (!$(this).val()) {
            const effectiveInput = $('input[name$=".EffectiveDate"]').eq(index);
            if (effectiveInput.val()) {
                const effectiveDate = new Date(effectiveInput.val());
                const expirationDate = new Date(effectiveDate);
                expirationDate.setFullYear(expirationDate.getFullYear() + 1);
                $(this).val(expirationDate.toISOString().split('T')[0]);
            }
        }
    });
}

// Format currency inputs
function formatCurrency() {
    $('input[type="number"][step="0.01"]').on('blur', function() {
        const value = parseFloat($(this).val());
        if (!isNaN(value)) {
            $(this).val(value.toFixed(2));
        }
    });
}

// Initialize on document ready
$(document).ready(function() {
    // Set default dates for new forms
    setDefaultDates();
    
    // Format currency inputs
    formatCurrency();
    
    // Add custom validation to the form
    $('form').on('submit', function(e) {
        // Only validate if it's the calculation form
        if ($(this).find('button[formaction*="Calculate"]').length) {
            if (!validateDates()) {
                e.preventDefault();
                return false;
            }
        }
    });
});