using Xamarin.Forms;
using AppName.Core;
using Realms;
using AppName.Model;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Behaviors;

namespace AppName
{
    /// <summary>
    /// ListView Item Tapped Behavior.
    /// </summary>
    public class ItemTappedBehavior : BehaviorBase<ListView>
    {
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command { get; set; }

        /// <inheritDoc />
        protected override void OnAttachedTo(ListView bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject.ItemTapped += OnItemTapped;
        }

        /// <inheritDoc />
        protected override void OnDetachingFrom(ListView bindable)
        {
            base.OnDetachingFrom(bindable);
            AssociatedObject.ItemTapped -= OnItemTapped;
        }

        void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (Command == null || e.Item == null) return;

            if (Command.CanExecute(e.Item))
                Command.Execute(e.Item);
        }
    }
}