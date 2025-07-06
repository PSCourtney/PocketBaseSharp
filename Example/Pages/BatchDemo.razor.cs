using Example.Models;
using Microsoft.AspNetCore.Components;
using PocketBaseSharp;
using PocketBaseSharp.Services;

namespace Example.Pages
{
    public partial class BatchDemo
    {
        [Inject]
        public PocketBase PocketBase { get; set; } = null!;

        private string todoName = "Sample Todo";
        private List<string> entryNames = new() { "First task", "Second task", "Third task" };
        private bool isProcessing = false;
        private string resultMessage = string.Empty;
        private bool isSuccess = false;
        private List<BatchResponseItem>? batchResults;

        private void AddEntry()
        {
            entryNames.Add($"New task {entryNames.Count + 1}");
        }

        private void RemoveEntry(int index)
        {
            if (index >= 0 && index < entryNames.Count)
            {
                entryNames.RemoveAt(index);
            }
        }

        private async Task ExecuteBatchSimple()
        {
            // This demonstrates a simple batch operation creating multiple todo items
            isProcessing = true;
            resultMessage = string.Empty;
            batchResults = null;
            StateHasChanged();

            try
            {
                // Create a batch builder
                var batchBuilder = PocketBase.Batch.CreateBatch();

                // Add multiple todo creations to demonstrate batch functionality
                for (int i = 0; i < entryNames.Count && i < 3; i++) // Limit to 3 for demo
                {
                    var todo = new Todo
                    {
                        name = $"{todoName} #{i + 1}"
                    };

                    batchBuilder.Create("todos", todo);
                }

                // Execute the batch
                var result = await batchBuilder.SendAsync();

                if (result.IsSuccess)
                {
                    batchResults = result.Value?.ToList();
                    var successCount = batchResults?.Count(r => r.Status == 200 || r.Status == 201) ?? 0;
                    var totalCount = batchResults?.Count ?? 0;
                    
                    resultMessage = $"Batch executed successfully! {successCount}/{totalCount} todos created.";
                    isSuccess = true;
                }
                else
                {
                    resultMessage = $"Batch execution failed: {result.Errors.FirstOrDefault()?.Message ?? "Unknown error"}";
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                resultMessage = $"Error executing batch: {ex.Message}";
                isSuccess = false;
            }
            finally
            {
                isProcessing = false;
                StateHasChanged();
            }
        }

        private async Task ExecuteBatchCorrectly()
        {
            // This demonstrates the correct approach - create todo first, then create entries with the actual todo ID
            isProcessing = true;
            resultMessage = string.Empty;
            batchResults = null;
            StateHasChanged();

            try
            {
                // Step 1: Create todo first to get its ID
                var todo = new Todo
                {
                    name = todoName
                };

                var todoResult = await PocketBase.Collection("todos").CreateAsync(todo);
                
                if (!todoResult.IsSuccess)
                {
                    resultMessage = $"Failed to create todo: {todoResult.Errors.FirstOrDefault()?.Message ?? "Unknown error"}";
                    isSuccess = false;
                    return;
                }

                var createdTodo = todoResult.Value;
                
                // Step 2: Create batch for entries
                var batchBuilder = PocketBase.Batch.CreateBatch();
                
                foreach (var entryName in entryNames.Where(name => !string.IsNullOrWhiteSpace(name)))
                {
                    var entry = new Entry
                    {
                        name = entryName,
                        is_done = false,
                        todo_id = createdTodo.Id // Now we have the actual todo ID
                    };

                    batchBuilder.Create("entry", entry);
                }

                // Execute the entries batch
                var entriesResult = await batchBuilder.SendAsync();

                if (entriesResult.IsSuccess)
                {
                    batchResults = entriesResult.Value?.ToList();
                    var successCount = batchResults?.Count(r => r.Status == 200 || r.Status == 201) ?? 0;
                    var totalCount = batchResults?.Count ?? 0;
                    
                    resultMessage = $"Complete flow executed successfully! Created 1 todo and {successCount}/{totalCount} entries.";
                    isSuccess = true;
                }
                else
                {
                    resultMessage = $"Failed to create entries: {entriesResult.Errors.FirstOrDefault()?.Message ?? "Unknown error"}";
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                resultMessage = $"Error executing batch: {ex.Message}";
                isSuccess = false;
            }
            finally
            {
                isProcessing = false;
                StateHasChanged();
            }
        }
    }
}