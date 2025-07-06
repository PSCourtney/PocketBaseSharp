using Example.Models;
using Microsoft.AspNetCore.Components;
using PocketBaseSharp;
using static MudBlazor.CategoryTypes;

namespace Example.Pages.SharedComponents
{
    public partial class TodoListEntries
    {
        [Parameter]
        public string? Id { get; set; }

        [Inject]
        public PocketBase PocketBase { get; set; } = null!;

        private List<Entry>? _entries;

        bool isediting = false;

        protected override async Task OnParametersSetAsync()
        {
            await LoadEntriesAsync();
            await base.OnParametersSetAsync();
        }

        private async Task LoadEntriesAsync()
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                return;
            }

            var result =
                await PocketBase.Collection("entry").GetFullListAsync<Entry>(filter: $"todo_id='{Id}'");
            if (result.IsSuccess)
            {
                _entries = result.Value.ToList();
            }
        }

        private async Task AddNewEntryAsync()
        {
            if (_entries is null || string.IsNullOrWhiteSpace(Id))
            {
                return;
            }

            var newEntry = new Entry
            {
                name = "New Entry",
                todo_id = Id,
                is_done = false,
            };

            _entries.Add(newEntry);
            StateHasChanged();
        }

        private void ToggleEdit()
        {
            isediting = !isediting;
        }

        private async Task Remove(Entry item)
        {
            if (_entries is null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(item.Id))
            {
                // Use a lower-level API call to delete the entry
                var result = await PocketBase.SendAsync<object>(
                    $"/api/collections/entry/records/{item.Id}",
                    HttpMethod.Delete
                );

                if (!result.IsSuccess)
                {
                    // Handle error (e.g., log or show a message)
                    return;
                }
            }

            _entries.Remove(item);
            StateHasChanged();
        }

        private async Task SaveAsync()
        {
            if (_entries is null)
            {
                return;
            }

            foreach (var item in _entries)
            {
                if (string.IsNullOrEmpty(item.Id))
                {
                    item.todo_id = Id;
                    var result = await PocketBase.Collection("entry").CreateAsync<Entry>(item);
                    if (result.IsSuccess)
                    {
                        item.Id = result.Value.Id; // Update the item with the new ID
                        isediting = false;
                    }
                    else
                    {
                        // Handle error (e.g., log or show a message)
                    }
                }
                else
                {
                    // Update existing entry
                    var result = await PocketBase.Collection("entry").UpdateAsync<Entry>(item);
                    if (result.IsSuccess)
                    {
                        isediting = false;
                    }
                }
            }

            StateHasChanged();
        }

        private void NavigateBack()
        {
            // Use NavigationManager to go back to the list page
            NavigationManager.NavigateTo("/");
        }
    }
}
