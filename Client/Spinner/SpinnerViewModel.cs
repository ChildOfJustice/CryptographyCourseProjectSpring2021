namespace Task_3.WPF_Control_Elements.Spinner
{
    public class SpinnerViewModel: Task_3.ViewModels.BaseViewModel
    {
        private int _canvasCenterShiftLeft;
        private int _canvasCenterShiftTop;
        
        public int CanvasCenterShiftLeft
        {
            get =>
                _canvasCenterShiftLeft;

            set
            {
                _canvasCenterShiftLeft = value;
                OnPropertyChanged(nameof(CanvasCenterShiftLeft));
            }
        }
        public int CanvasCenterShiftTop
        {
            get =>
                _canvasCenterShiftTop;

            set
            {
                _canvasCenterShiftTop = value;
                OnPropertyChanged(nameof(CanvasCenterShiftTop));
            }
        }
        
        // _canvasCenterShiftLeft = -(xCenter + Size / 2);
        // _canvasCenterShiftTop = -(yCenter + Size / 2);
    }
}