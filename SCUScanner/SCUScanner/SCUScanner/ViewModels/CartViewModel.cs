using Plugin.Messaging;
using Plugin.Share;
using Plugin.Share.Abstractions;
using ReactiveUI;
using SCUScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SCUScanner.ViewModels
{
    public class CartsViewModel : BaseViewModel
    {
        public ObservableCollection<CartItemViewModel> Carts { get; set; } = new ObservableCollection<CartItemViewModel>();
        //private ICommand _clear;
        public ICommand ClearCommand { get; }
        public ICommand ShareCommand { get; }
        public ICommand SendEmailCommand { get; }
        private  async void DoShare()
        {
            var shareText = App.analizeSpare.GenerateResultSTR(GetPartList());
            await CrossShare.Current.Share(new ShareMessage
            {
                Title =string.Format(Resources["SharePartTitle"],App.SerialNumber),
                Text = shareText
                

            });
        }
        bool   isHasCart = false;
        public bool IsHasCart
        {

            get => isHasCart;
            set => this.RaiseAndSetIfChanged(ref isHasCart, value);
        }
        public List<Cart> GetPartList()
        {

            var parts1 = from p in Carts
                         select new Cart()
                         {
                             Part = p.Part,
                             Count = p.PartCount
                         };
            return parts1.ToList();
        }
        public CartsViewModel()
        {
            ClearCommand = ReactiveCommand.Create(() => {
                Carts.Clear();
                IsHasCart = false;
            },this.WhenAnyValue(vm=>vm.IsHasCart));
            ShareCommand = ReactiveCommand.Create(() => {
                DoShare();
            }, this.WhenAnyValue(vm => vm.IsHasCart));
            SendEmailCommand = ReactiveCommand.Create(() => {
                DoSendEmail();
            }, this.WhenAnyValue(vm => vm.IsHasCart));
        }

        private async void DoSendEmail()
        {
            var shareText = App.analizeSpare.GenerateResultSTR(GetPartList());
            var emailMessenger = CrossMessaging.Current.EmailMessenger;
            if (emailMessenger.CanSendEmail)
            {
                var email = new EmailMessageBuilder()
                        .To(App.analizeSpare.GetEmailTo())
                        .Subject(string.Format(Resources["SharePartTitle"], App.SerialNumber))
                        .Body(shareText)
                       .Build();
                emailMessenger.SendEmail(email);
            }
            else
                await App.Dialogs.AlertAsync("Can`t send email!!!");

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
            cart.Thump = App.analizeSpare.GetThump(part.PartNumber);
            cart.OnPartZeroOrLess += (s, a) =>
            {
                var obj = s as CartItemViewModel;
                Carts.Remove(obj);
            };
            Carts.Add(cart);
            IsHasCart = true;
        }
        public int TotalSum()
        {
            return Carts.Sum(p => p.PartCount);
        }
        
    }

    public class CartItemViewModel : BaseViewModel
    {
        public event EventHandler  OnPartZeroOrLess ;
        public ICommand DecCommand { get; }

        public ICommand IncCommand { get; }
        public ImageSource Thump { get; set; }
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
