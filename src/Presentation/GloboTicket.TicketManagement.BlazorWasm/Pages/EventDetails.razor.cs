﻿using GloboTicket.TicketManagement.Application.Contracts.Infrastructure.Services;
using GloboTicket.TicketManagement.Application.Features.Categories.Commands.CreateCateogry;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesList;
using GloboTicket.TicketManagement.Application.Features.Events.Queries.GetEventDetail;
using GloboTicket.TicketManagement.Application.Responses;
using GloboTicket.TicketManagement.BlazorWasm.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace GloboTicket.TicketManagement.BlazorWasm.Pages
{
    public partial class EventDetails
    {
        [Inject]
        public IEventDataService EventDataService { get; set; }

        [Inject]
        public ICategoryDataService CategoryDataService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public EventDetailVm EventDetailViewModel { get; set; } 
            = new() { Date = DateTime.Now.AddDays(1) };

        public ObservableCollection<CategoryListVm> Categories { get; set; } 
            = new ObservableCollection<CategoryListVm>();

        public string Message { get; set; }
        public string SelectedCategoryId { get; set; }

        [Parameter]
        public string EventId { get; set; }
        private Guid SelectedEventId = Guid.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (Guid.TryParse(EventId, out SelectedEventId))
            {
                EventDetailViewModel = await EventDataService.GetEventById(SelectedEventId);
                SelectedCategoryId = EventDetailViewModel.CategoryId.ToString();
                //return;
            }

            var list = await CategoryDataService.GetAllCategories();
            Categories = new ObservableCollection<CategoryListVm>(list);
            //SelectedCategoryId = Categories.FirstOrDefault().CategoryId.ToString();
        }

        protected async Task HandleValidSubmit()
        {
            EventDetailViewModel.CategoryId = Guid.Parse(SelectedCategoryId);
            ApiResponse<Guid> response;

            if (SelectedEventId == Guid.Empty)
            {
                response = await EventDataService.CreateEvent(EventDetailViewModel);
            }
            else
            {
                 response = await EventDataService.UpdateEvent(EventDetailViewModel);
            }
            HandleResponse(response);

        }

        protected async Task DeleteEvent()
        {
            var response = await EventDataService.DeleteEvent(SelectedEventId);
            HandleResponse(response);
        }

        protected void NavigateToOverview()
        {
            NavigationManager.NavigateTo("/eventoverview");
        }

        private void HandleResponse(ApiResponse<Guid> response)
        {
            if (response.Success)
            {
                NavigationManager.NavigateTo("/eventoverview");
            }
            else
            {
                Message = response.Message;
                if (!string.IsNullOrEmpty(response.ValidationErrors))
                    Message += response.ValidationErrors;
            }
        }
    }
}
