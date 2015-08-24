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

//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.widget.BaseAdapter;

//import com.nhaarman.listviewanimations.util.Insertable;
//import com.nhaarman.listviewanimations.util.Swappable;

//import java.util.ArrayList;
//import java.util.Collection;
//import java.util.List;

/**
 * A true {@link ArrayList} adapter providing access to some of the {@code ArrayList} methods.
 * <p/>
 * Also implements {@link Swappable} for easy object swapping,
 * and {@link com.nhaarman.listviewanimations.util.Insertable} for inserting objects.
 */
//@SuppressWarnings("UnusedDeclaration")

using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Util;
using System;
using Java.Lang;
using System.Collections.Generic;
namespace Com.Nhaarman.ListviewAnimations
{
    public abstract class ArrayAdapter<T> : BaseAdapter, ISwappable, IInsertable
    {

        //@NonNull
        private IList<T> mItems;

        private BaseAdapter mDataSetChangedSlavedAdapter;

        /**
         * Creates a new ArrayAdapter with an empty {@code List}.
         */
        protected ArrayAdapter()
            : this(null)
        {
            //this(null);
        }

        /**
         * Creates a new ArrayAdapter, using (a copy of) given {@code List}, or an empty {@code List} if objects = null.
         */

        protected ArrayAdapter(IList<T> objects)
        {
            //protected ArrayAdapter(@Nullable final List<T> objects) {
            if (objects != null)
            {
                mItems = objects;
            }
            else
            {
                mItems = new List<T>();
            }
        }

        //@Override
        public override int Count
        {
            get { return mItems.Count; }
        }

        //public int getCount() {
        //    return mItems.size();
        //}

        public override long GetItemId(int position)
        {
            return position;
        }

        //@Override
        //public long getItemId(final int position) {
        //    return position;
        //}     

        public override Java.Lang.Object GetItem(int position)
        {
                T item = mItems[position];
                return item as Java.Lang.Object; 
        }

        public T GetItem2(int position)
        {
            return mItems[position] ;
        }
        

        //@Override
        //@NonNull
        //public T getItem(final int position) {
        //    return mItems.get(position);
        //}

        /**
         * Returns the items.
         */
        //@NonNull
        public IList<T> getItems()
        {
            return mItems;
        }



        /**
         * Appends the specified element to the end of the {@code List}.
         *
         * @param object the object to add.
         *
         * @return always true.
         */
        public bool add(T anobject)
        {
            bool result = true;
            mItems.Add(anobject);
            notifyDataSetChanged();
            return result;
        }

        //@Override

        public void add(int index, object item)
        {
            mItems.Insert(index, (T)item);
            //mItems.Add(index, item);
            notifyDataSetChanged();
        }

        /**
         * Adds the objects in the specified collection to the end of this List. The objects are added in the order in which they are returned from the collection's iterator.
         *
         * @param collection the collection of objects.
         *
         * @return {@code true} if this {@code List} is modified, {@code false} otherwise.
         */
        //public bool addAll(@NonNull final Collection<? extends T> collection) {
        //    boolean result = mItems.addAll(collection);
        //    notifyDataSetChanged();
        //    return result;
        //}

        public bool addAll(ICollection<T> collection)
        {

            bool result = true;// mItems.addAll(collection);
            foreach (T item in collection)
            {
                mItems.Add(item);
            }

            notifyDataSetChanged();
            return result;
        }

        public bool contains(T anobject)
        {
            return mItems.Contains(anobject);
        }

        public void Clear()
        {
            mItems.Clear();
            notifyDataSetChanged();
        }

        public bool Remove(T anobject)
        {
            bool result = mItems.Remove(anobject);
            notifyDataSetChanged();
            return result;
        }

        //@NonNull
        public T Remove(int location) 
        {           
            T result = mItems[location];
            mItems.RemoveAt(location);
            notifyDataSetChanged();
            return result;
        }

        //@Override
        public void swapItems(int positionOne, int positionTwo)
        {             
            T firstitem = mItems[positionOne];
            mItems[positionOne] = mItems[positionTwo];
            //T firstItem = mItems.set(positionOne, getItem(positionTwo));
            notifyDataSetChanged();
            //mItems.set(positionTwo, firstItem);
            mItems[positionTwo] = firstitem;
        }

        public void propagateNotifyDataSetChanged(BaseAdapter slavedAdapter)
        {
            mDataSetChangedSlavedAdapter = slavedAdapter;
        }

        //@Override
        public void notifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
            if (mDataSetChangedSlavedAdapter != null)
            {
                mDataSetChangedSlavedAdapter.NotifyDataSetChanged();
            }
        }
    }
}