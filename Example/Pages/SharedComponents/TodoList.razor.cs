using Example.Models;
using Microsoft.AspNetCore.Components;
using PocketBaseSharp;

namespace Example.Pages.SharedComponents
{
    public partial class TodoList
    {
        [Inject]
        public PocketBase PocketBase { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        private bool _loading = false;
        private IEnumerable<Todo>? todos;

        private Todo? editTodo;
        private string? editTodoName;
        private bool editTodoChanged => editTodo != null && editTodoName != editTodo.name && !string.IsNullOrWhiteSpace(editTodoName);

        private bool creatingNewTodo = false;
        private string? newTodoName;

        protected override async Task OnInitializedAsync()
        {
            await LoadTodosFromPocketbase();
            await base.OnInitializedAsync();
        }

        private async Task LoadTodosFromPocketbase()
        {
            _loading = true;

            var result = await PocketBase.Collection("todos").GetFullListAsync<Todo>();

            if (result.IsSuccess)
            {
                todos = result.Value;
            }

            _loading = false;
        }

        private async Task CreateNewTodoAsync()
        {
            var newTodo = new Todo
            {
                name = $"New List {DateTime.Now:HHmmss}"
            };
            var result = await PocketBase.Collection("todos").CreateAsync<Todo>(newTodo);
            if (result.IsSuccess)
            {
                await LoadTodosFromPocketbase();
            }
            // Optionally handle errors here
        }

        private void GoToDetails(Todo dto)
        {
            var path = $"/details/{dto.Id}";
            NavigationManager.NavigateTo(path);
        }

        private void StartEditTodo(Todo todo)
        {
            editTodo = todo;
            editTodoName = todo.name;
        }

        private void CancelEditTodo()
        {
            editTodo = null;
            editTodoName = null;
        }

        private async Task SaveEditTodoAsync()
        {
            if (editTodo != null && editTodoChanged)
            {
                editTodo.name = editTodoName;
                var result = await PocketBase.Collection("todos").UpdateAsync<Todo>(editTodo);
                if (result.IsSuccess)
                {
                    await LoadTodosFromPocketbase();
                    editTodo = null;
                    editTodoName = null;
                }
                // Optionally handle errors
            }
        }

        private void StartNewTodo()
        {
            creatingNewTodo = true;
            newTodoName = string.Empty;
        }

        private void CancelNewTodo()
        {
            creatingNewTodo = false;
            newTodoName = null;
        }

        private async Task SaveNewTodoAsync()
        {
            if (!string.IsNullOrWhiteSpace(newTodoName))
            {
                var newTodo = new Todo
                {
                    name = newTodoName
                };
                var result = await PocketBase.Collection("todos").CreateAsync<Todo>(newTodo);
                if (result.IsSuccess)
                {
                    await LoadTodosFromPocketbase();
                    creatingNewTodo = false;
                    newTodoName = null;
                }
                // Optionally handle errors
            }
        }
    }
}
