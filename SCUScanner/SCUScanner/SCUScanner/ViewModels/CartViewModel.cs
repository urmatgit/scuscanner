using ReactiveUI;
using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SCUScanner.ViewModels
{
    public class CartsViewModel : BaseViewModel
    {
        public ObservableCollection<CartItemViewModel> Carts { get; set; } = new ObservableCollection<CartItemViewModel>();
        //private ICommand _clear;
        public ICommand ClearCommand { get; }
        public ICommand ShareCommand { get; }
        
        private void DoShare()
        {
            var shareText = App.analizeSpare.GenerateResultSTR(GetPartList());
        }
        public List<Cart> GetPartList()
        {

            var parts1 = from p in Carts
                         select new Cart()
                         {
                             Part = p.Part,
                             Count = p.PartCount,
                             Thump = App.analizeSpare.GetThump(p.Part.PartNumber)
                         };
            return parts1.ToList();
        }
        public CartsViewModel()
        {
            ClearCommand = ReactiveCommand.Create(() => {
                Carts.Clear();
            });
            ShareCommand = ReactiveCommand.Create(() => {
                DoShare();
            });
        }
        public void AddCart(Part part)
        {
            var finded = Carts.FirstOrDefault(p => p.Part.PartNumber == part.PartNumber);
            if (finded != null)
            {
                finded.PartCount++;
                return;
            }
            var cart = new CartItemViewModel(part);
            cart.OnPartZeroOrLess += (s, a) =>
            {
                var obj = s as CartItemViewModel;
                Carts.Remove(obj);
            };
            Carts.Add(cart);
        }
        
    }

    public class CartItemViewModel : BaseViewModel
    {
        public event EventHandler  OnPartZeroOrLess ;
        public ICommand DecCommand { get; }

        public ICommand IncCommand { get; }

        public Part Part { get; set; }

        private int partCount;
        public int PartCount
        {
            get { return partCount; }
            set
            {

                this.RaiseAndSetIfChanged(ref partCount, value);
                if (partCount <= 0)
                {
                    if (OnPartZeroOrLess != null)
                        OnPartZeroOrLess(this, new EventArgs());
                }
            }
        }

        public CartItemViewModel(Part part)
        {
            DecCommand = ReactiveCommand.Create(() =>
            {
                PartCount--;
            });
            IncCommand = ReactiveCommand.Create(() =>
            {
                PartCount++;
            });
            Part = part;
            PartCount = 1;
        }
      
            
    }
}
