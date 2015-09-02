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

//package com.haarman.listviewanimations;

//import android.content.Context;
//import android.support.annotation.NonNull;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.TextView;

//import com.nhaarman.listviewanimations.ArrayAdapter;
//import com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.undo.UndoAdapter;

using Android.Content;
using Android.Views;
using Android.Widget;
using com.refractored.components.stickylistheaders;
using Com.Nhaarman.ListviewAnimations;
using Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss.undo;
//import se.emilsjolander.stickylistheaders.StickyListHeadersAdapter;
//StickyListHeadersAdapter
//using Android.Widget;
namespace ListviewAnimations.Sample
{
    public class MyListAdapter : Com.Nhaarman.ListviewAnimations.ArrayAdapter<string>, IUndoAdapter, IStickyListHeadersAdapter
    {

        private Context mContext;

         

        public MyListAdapter(Context context)
        {
            mContext = context;
            for (int i = 0; i < 1000; i++)
            {
                add(mContext.GetString(Resource.String.row_number, i));
            }
        }

        //@Override
        public long getItemId(int position)
        {
            return GetItem2(position).GetHashCode();
        }

        //@Override
        public bool hasStableIds()
        {
            return true;
        }

        //@Override
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            TextView view = (TextView)convertView;
            if (view == null)
            {
                view = (TextView)LayoutInflater.From(mContext).Inflate(Resource.Layout.list_row, parent, false);
            }

            view.Text=GetItem2(position);

            return view;
        }

        //@NonNull
        //@Override
        public View getUndoView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(mContext).Inflate(Resource.Layout.undo_row, parent, false);
            }
            return view;
        }

        //@NonNull
        //@Override
        public View getUndoClickView(View view)
        {
            return view.FindViewById(Resource.Id.undo_row_undobutton);
        }

        //@Override
        public View GetHeaderView(int position, View convertView, ViewGroup parent)
        {
            TextView view = (TextView)convertView;
            if (view == null)
            {
                view = (TextView)LayoutInflater.From(mContext).Inflate(Resource.Layout.list_header, parent, false);
            }

            view.Text=mContext.GetString(Resource.String.header, GetHeaderId(position));

            return view;
        }

        //@Override
        public long GetHeaderId(int position)
        {
            return position / 10;
        }
    }
}