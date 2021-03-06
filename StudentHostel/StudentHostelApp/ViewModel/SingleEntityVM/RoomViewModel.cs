﻿using StudentHostelApp.Code;

namespace StudentHostelApp.ViewModel.SingleEntityVM
{
    /// <summary>
    /// Предоставляет информацию о комнате для отображения
    /// </summary>
    public class RoomViewModel:BaseViewModel
    {
        private int roomId;
        public int RoomId
        {
            get { return this.roomId; }
            set { this.roomId = value; OnPropertyChanged(nameof(RoomId)); }
        }
        private string roomNo;
        public string RoomNo
        {
            get { return this.roomNo; }
            set { this.roomNo = value; OnPropertyChanged(nameof(RoomNo)); }
        }
        private byte seats;
        public byte Seats
        {
            get { return this.seats; }
            set { this.seats = value; OnPropertyChanged(nameof(Seats)); }
        }

        // Количество свободных мест в комнате
        private int freeSeats;
        public int FreeSeats
        {
            get { return this.freeSeats; }
            set { this.freeSeats = value; OnPropertyChanged(nameof(FreeSeats)); }
        }
    }
}
