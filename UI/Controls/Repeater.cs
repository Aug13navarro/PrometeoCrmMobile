using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace UI.Controls
{
    // Tomado de: https://forums.xamarin.com/discussion/87128/repeaterview
    public class Repeater : StackLayout
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            "ItemSource",
            typeof(IEnumerable),
            typeof(Repeater),
            new List<object>(),
            BindingMode.OneWay,
            propertyChanged: ItemsChanged);

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(Repeater),
            default(DataTemplate));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public Repeater()
        {
            Spacing = 0;
        }

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (Repeater)bindable;

            if (oldValue is INotifyCollectionChanged oldObservableCollection)
            {
                oldObservableCollection.CollectionChanged -= control.OnItemsSourceCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged newObservableCollection)
            {
                newObservableCollection.CollectionChanged += control.OnItemsSourceCollectionChanged;
            }

            control.Children.Clear();

            if (newValue != null)
            {
                foreach (object item in (IEnumerable)newValue)
                {
                    View view = control.CreateChildViewFor(item);
                    control.Children.Add(view);
                }
            }

            control.UpdateChildrenLayout();
            control.InvalidateLayout();
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var invalidate = false;

            if (e.OldItems != null)
            {
                Children.RemoveAt(e.OldStartingIndex);
                invalidate = true;
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; ++i)
                {
                    var item = e.NewItems[i];
                    var view = CreateChildViewFor(item);

                    Children.Insert(i + e.NewStartingIndex, view);
                }

                invalidate = true;
            }

            if (invalidate)
            {
                UpdateChildrenLayout();
                InvalidateLayout();
            }
        }

        private View CreateChildViewFor(object item)
        {
            ItemTemplate.SetValue(BindingContextProperty, item);
            return (View)ItemTemplate.CreateContent();
        }
    }
}
