/*
 * Copyright 2014 Niek Haarman
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
//package com.nhaarman.listviewanimations;

//import android.database.DataSetObserver;
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.util.Log;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.AbsListView;
//import android.widget.BaseAdapter;
//import android.widget.SectionIndexer;

//import com.nhaarman.listviewanimations.util.AbsListViewWrapper;
//import com.nhaarman.listviewanimations.util.Insertable;
//import com.nhaarman.listviewanimations.util.ListViewWrapper;
//import com.nhaarman.listviewanimations.util.ListViewWrapperSetter;
//import com.nhaarman.listviewanimations.util.Swappable;

using Android.Database;
using Android.Util;
using Android.Views;
/**
 * A decorator class that enables decoration of an instance of the {@link BaseAdapter} class.
 * <p/>
 * Classes extending this class can override methods and provide extra functionality before or after calling the super method.
 */
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Util;
namespace Com.Nhaarman.ListviewAnimations
{
public abstract class BaseAdapterDecorator : BaseAdapter , ISectionIndexer, Swappable, Insertable, ListViewWrapperSetter {

    /**
     * The {@link android.widget.BaseAdapter} this {@code BaseAdapterDecorator} decorates.
     */
    //@NonNull
    private BaseAdapter mDecoratedBaseAdapter;

    /**
     * The {@link com.nhaarman.listviewanimations.util.ListViewWrapper} containing the ListView this {@code BaseAdapterDecorator} will be bound to.
     */
    //@Nullable
    private ListViewWrapper mListViewWrapper;

    /**
     * Create a new {@code BaseAdapterDecorator}, decorating given {@link android.widget.BaseAdapter}.
     *
     * @param baseAdapter the {@code} BaseAdapter to decorate.
     */
    protected BaseAdapterDecorator(BaseAdapter baseAdapter) {
        mDecoratedBaseAdapter = baseAdapter;
    }

    /**
     * Returns the {@link android.widget.BaseAdapter} that this {@code BaseAdapterDecorator} decorates.
     */
    //@NonNull
    public BaseAdapter getDecoratedBaseAdapter() {
        return mDecoratedBaseAdapter;
    }

    /**
     * Returns the root {@link android.widget.BaseAdapter} this {@code BaseAdapterDecorator} decorates.
     */
    //@NonNull
    protected BaseAdapter getRootAdapter() {
        BaseAdapter adapter = mDecoratedBaseAdapter;
        while (adapter is BaseAdapterDecorator) {
            adapter = ((BaseAdapterDecorator) adapter).getDecoratedBaseAdapter();
        }
        return adapter;
    }

    public void setAbsListView(AbsListView absListView) {
        setListViewWrapper(new AbsListViewWrapper(absListView));
    }

    /**
     * Returns the {@link com.nhaarman.listviewanimations.util.ListViewWrapper} containing the ListView this {@code BaseAdapterDecorator} is bound to.
     */
    //@Nullable
    public ListViewWrapper getListViewWrapper() {
        return mListViewWrapper;
    }

    /**
     * Alternative to {@link #setAbsListView(android.widget.AbsListView)}. Sets the {@link com.nhaarman.listviewanimations.util.ListViewWrapper} which contains the ListView
     * this adapter will be bound to. Call this method before setting this adapter to the ListView. Also propagates to the decorated {@code BaseAdapter} if applicable.
     */
    //@Override
    public virtual void setListViewWrapper(ListViewWrapper listViewWrapper) {
        mListViewWrapper = listViewWrapper;

        if (mDecoratedBaseAdapter is ListViewWrapperSetter) {
            ((ListViewWrapperSetter) mDecoratedBaseAdapter).setListViewWrapper(listViewWrapper);
        }
    }

    //@Override
    //public int getCount() {
    //    return mDecoratedBaseAdapter.getCount();
    //}

    public override int Count
    {
        get { return mDecoratedBaseAdapter.Count; }
    }

    //@Override
    //public Object getItem(int position) {
    //    return mDecoratedBaseAdapter.getItem(position);
    //}

    public override Java.Lang.Object GetItem(int position)
    {
        return mDecoratedBaseAdapter.GetItem(position);
    }

    //@Override
    public override long GetItemId(int position)
    {
        return mDecoratedBaseAdapter.GetItemId(position);
    }

    //@Override
    //@NonNull
    //public View getView(int position, View convertView, ViewGroup parent) {
    //    return mDecoratedBaseAdapter.GetView(position, convertView, parent);
    //}

    public override View GetView(int position, View convertView, ViewGroup parent)
    {
        return mDecoratedBaseAdapter.GetView(position, convertView, parent);
    }

    //@Override
    public override bool AreAllItemsEnabled() {
        return mDecoratedBaseAdapter.AreAllItemsEnabled();        
    }

    //@Override
    //@NonNull
    public override View GetDropDownView(int position, View convertView, ViewGroup parent)
    {
        return mDecoratedBaseAdapter.GetDropDownView(position, convertView, parent);
    }

    //@Override
    public override int GetItemViewType(int position)
    {
        return mDecoratedBaseAdapter.GetItemViewType(position);
    }

    

    //@Override
    //public override int getViewTypeCount() {
    //    return mDecoratedBaseAdapter.ViewTypeCount;        
    //}

    public override int ViewTypeCount
    {
        get
        {
            return mDecoratedBaseAdapter.ViewTypeCount;   
        }
    }


    //@Override
    //public override bool hasStableIds() {
    //    return mDecoratedBaseAdapter.HasStableIds;
    //}

    public override bool HasStableIds
    {
        get
        {
            return mDecoratedBaseAdapter.HasStableIds;
        }
    }

    //@Override
    //public override bool isEmpty() {
    //    return mDecoratedBaseAdapter.IsEmpty;
    //}

    public override bool IsEmpty
    {
        get
        {
            return mDecoratedBaseAdapter.IsEmpty;
        }
    }

    //@Override
    public override bool IsEnabled(int position)
    {
        return mDecoratedBaseAdapter.IsEnabled(position);
    }     

    //@Override
    public override void NotifyDataSetChanged()
    {
        if (!(mDecoratedBaseAdapter is ArrayAdapter)) {
            // fix #35 dirty trick !
            // leads to an infinite loop when trying because ArrayAdapter triggers notifyDataSetChanged itself
            // TODO: investigate
            mDecoratedBaseAdapter.NotifyDataSetChanged();
        }
    }

    /**
     * Helper function if you want to force notifyDataSetChanged()
     */
    //@SuppressWarnings("UnusedDeclaration")
    public void notifyDataSetChanged(bool force) {
        if (force || !(mDecoratedBaseAdapter is ArrayAdapter)) {
            // leads to an infinite loop when trying because ArrayAdapter triggers notifyDataSetChanged itself
            // TODO: investigate
            mDecoratedBaseAdapter.NotifyDataSetChanged();
        }
    }

    //@Override
    public override void NotifyDataSetInvalidated()
    {
        mDecoratedBaseAdapter.NotifyDataSetInvalidated();
    }

    //@Override
    public override void RegisterDataSetObserver(DataSetObserver observer) {
        mDecoratedBaseAdapter.RegisterDataSetObserver(observer);
    }

    //@Override
    public override void UnregisterDataSetObserver(DataSetObserver observer) {
        mDecoratedBaseAdapter.UnregisterDataSetObserver(observer);
    }

    //@Override
    public int GetPositionForSection(int sectionIndex) {
        int result = 0;
        if (mDecoratedBaseAdapter is ISectionIndexer) {
            result = ((ISectionIndexer) mDecoratedBaseAdapter).GetPositionForSection(sectionIndex);
        }
        return result;
        
    }

    //@Override
    public int GetSectionForPosition(int position) {
        int result = 0;
        if (mDecoratedBaseAdapter is ISectionIndexer) {
            result = ((ISectionIndexer) mDecoratedBaseAdapter).GetSectionForPosition(position);
        }
        return result;
    }

    //@Override
    //@NonNull
    public Java.Lang.Object[] GetSections() {
        Java.Lang.Object[] result = new Java.Lang.Object[0];
        if (mDecoratedBaseAdapter is ISectionIndexer) {
            result = ((ISectionIndexer) mDecoratedBaseAdapter).GetSections();
        }
        return result;
    }

    //@Override
    public void swapItems(int positionOne, int positionTwo) {
        if (mDecoratedBaseAdapter is Swappable) {
            ((Swappable) mDecoratedBaseAdapter).swapItems(positionOne, positionTwo);
        } else {
            Log.Warn("ListViewAnimations", "Warning: swapItems called on an adapter that does not implement Swappable!");
        }
    }

    //@Override
    public void add(int index, object item) {
        if (mDecoratedBaseAdapter is Insertable) {
            //noinspection rawtypes
            ((Insertable) mDecoratedBaseAdapter).add(index, item);
        } else {
            Log.Warn("ListViewAnimations", "Warning: add called on an adapter that does not implement Insertable!");
        }
    }
}
}