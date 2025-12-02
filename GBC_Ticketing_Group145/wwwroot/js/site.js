// Function to update the total cost in real-time on the Purchase/Buy page
function update_total_cost() {
    // 1. Get the current ticket price from a hidden input or an element on the page
    // Assuming you have a hidden input with the Event's TicketPrice
    const priceElement = document.getElementById('PricePerTicket');

    // Fallback: If you don't have a hidden input, you must get the price from the view content
    // For simplicity, let's assume the price is accessible globally or in a data attribute

    // --- MANDATED PART 1: Get Price and Quantity ---
    // If you don't have a hidden input, you may need to pass the price via a data attribute on the Quantity field.
    // In this example, we assume the price is stored in a hidden input field named 'PricePerTicket'.
    const priceInput = document.getElementById('PricePerTicket');
    if (!priceInput) {
        // If the hidden input isn't found, stop the function.
        // The purchase still works via the server, this is just for UX.
        return;
    }

    const price = parseFloat(priceInput.value);
    const quantity = parseInt(document.getElementById('Quantity').value);

    // 2. Perform validation and calculation
    if (isNaN(price) || isNaN(quantity) || quantity < 1) {
        document.getElementById('TotalCostDisplay').textContent = 'N/A';
        return;
    }

    // --- MANDATED PART 2: Calculate and Format ---
    const totalCost = price * quantity;

    // Use the browser's native currency formatter for clean display
    const formattedCost = totalCost.toLocaleString('en-US', {
        style: 'currency',
        currency: 'USD'
    });

    // 3. Update the display element
    document.getElementById('TotalCostDisplay').textContent = formattedCost;
}

// --- MANDATED PART 3: Attach Event Listeners ---
// Run the calculation function when the page loads, and every time the quantity changes.
document.addEventListener('DOMContentLoaded', function() {
    const quantityInput = document.getElementById('Quantity');
    if (quantityInput) {
        // Attach the function to run whenever the input value changes
        quantityInput.addEventListener('input', update_total_cost);

        // Run the function once on page load to initialize the display
        update_total_cost();
    }
});