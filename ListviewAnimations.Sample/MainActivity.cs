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

//import android.annotation.SuppressLint;
//import android.app.Activity;
//import android.app.PendingIntent;
//import android.content.ComponentName;
//import android.content.Context;
//import android.content.Intent;
//import android.content.IntentSender.SendIntentException;
//import android.content.ServiceConnection;
//import android.net.Uri;
//import android.os.Build;
//import android.os.Bundle;
//import android.os.IBinder;
//import android.os.RemoteException;
//import android.view.Menu;
//import android.view.MenuItem;
//import android.view.View;
//import android.view.WindowManager;
//import android.widget.Toast;

////import com.android.vending.billing.IInAppBillingService;
//import com.haarman.listviewanimations.appearance.AppearanceExamplesActivity;
//import com.haarman.listviewanimations.googlecards.GoogleCardsActivity;
//import com.haarman.listviewanimations.gridview.GridViewActivity;
//import com.haarman.listviewanimations.itemmanipulation.ItemManipulationsExamplesActivity;

//import org.json.JSONException;
//import org.json.JSONObject;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using ListviewAnimations.Sample.appearance;
using ListviewAnimations.Sample.gridview;
using ListviewAnimations.Sample.itemmanipulation;



namespace ListviewAnimations.Sample
{
    [Activity(Label = "ListviewAnimations(.Net)", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        private static readonly string URL_GITHUB_IO = "http://nhaarman.github.io/ListViewAnimations?ref=app";

        private readonly IServiceConnection mServiceConn = new MyServiceConnection();
        //  private IInAppBillingService mService;

        //@SuppressLint("InlinedApi")
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                Window.AddFlags(WindowManagerFlags.TranslucentNavigation);                
            }

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            BindService(new Intent("com.android.vending.billing.InAppBillingService.BIND"), mServiceConn, Bind.AutoCreate);

            FrameLayout flgv = FindViewById<FrameLayout>(Resource.Id.GridViewExample);
            flgv.Click += delegate {
                Intent intent = new Intent(this, typeof(GridViewActivity));
                StartActivity(intent);
            };

            FrameLayout flapp = FindViewById<FrameLayout>(Resource.Id.Appearance);
            flapp.Click += delegate
            {
                Intent intent = new Intent(this, typeof(AppearanceExamplesActivity));
                StartActivity(intent);
            };

            FrameLayout flitem = FindViewById<FrameLayout>(Resource.Id.ItemManipulation);
            flitem.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ItemManipulationsExamplesActivity));
                StartActivity(intent);
            };

        }

        //@Override
        public bool onCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            //menu.findItem(R.id.menu_main_donate).setVisible(mService != null);

            return base.OnCreateOptionsMenu(menu);
        }

        //@Override
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_main_github:
                    Intent intent = new Intent(Intent.ActionView);
                    intent.SetData(Uri.Parse(URL_GITHUB_IO));
                    StartActivity(intent);
                    return true;
                case Resource.Id.menu_main_beer:
                    buy("beer");
                    return true;
                case Resource.Id.menu_main_beer2:
                    buy("beer2");
                    return true;
                case Resource.Id.menu_main_beer3:
                    buy("beer3");
                    return true;
                case Resource.Id.menu_main_beer4:
                    buy("beer4");
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void onGoogleCardsExampleClicked(View view)
        {
            //Intent intent = new Intent(this, GoogleCardsActivity.class);
            //StartActivity(intent);
        }        

        public void onGridViewExampleClicked(View view)
        {
            Intent intent = new Intent(this, typeof(GridViewActivity));
            StartActivity(intent);
        }

        public void onAppearanceClicked(View view)
        {
            Intent intent = new Intent(this, typeof(AppearanceExamplesActivity));
            StartActivity(intent);
        }

        public void onItemManipulationClicked(View view)
        {
            Intent intent = new Intent(this, typeof(ItemManipulationsExamplesActivity));
            StartActivity(intent);
        }

        public void onSLHClicked(View view)
        {
            //Intent intent = new Intent(this, StickyListHeadersActivity.class);
            //startActivity(intent);
        }



        //@Override
        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnbindService(mServiceConn);
        }

        //@Override
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                Toast.MakeText(this, GetString(Resource.String.thanks), ToastLength.Long).Show();

                new Java.Lang.Thread(new ConsumePurchaseRunnable(data)).Start();
            }
        }

        private void buy(string sku)
        {
            /*try {
                Bundle buyIntentBundle = mService.getBuyIntent(3, getPackageName(), sku, "inapp", "bGoa+V7g/ysDXvKwqq+JTFn4uQZbPiQJo4pf9RzJ");
                PendingIntent pendingIntent = buyIntentBundle.getParcelable("BUY_INTENT");
                if (pendingIntent != null) {
                    startIntentSenderForResult(pendingIntent.getIntentSender(), 1001, new Intent(), 0, 0, 0);
                }
            } catch (RemoteException | SendIntentException ignored) {
                Toast.makeText(this, getString(R.string.exception), Toast.LENGTH_LONG).show();
            }*/
        }

        private class MyServiceConnection : Java.Lang.Object, IServiceConnection
        {
            ////@Override
            //public void onServiceDisconnected(ComponentName name) {
            //    //mService = null;
            //}

            ////@Override
            //public void onServiceConnected(ComponentName name, IBinder service) {
            //    //mService = IInAppBillingService.Stub.asInterface(service);

            //    new Thread(new RetrievePurchasesRunnable()).start();
            //}
            public void OnServiceConnected(ComponentName name, IBinder service)
            {
                //mService = null;
            }

            public void OnServiceDisconnected(ComponentName name)
            {
                //mService = IInAppBillingService.Stub.asInterface(service);

                //    new Thread(new RetrievePurchasesRunnable()).start();
            }
        }

        private class RetrievePurchasesRunnable : Java.Lang.Object, Java.Lang.IRunnable
        {
            //@Override
            public void Run()
            {
                /*try {
                    Bundle ownedItems = mService.getPurchases(3, getPackageName(), "inapp", null);

                    int response = ownedItems.getInt("RESPONSE_CODE");
                    if (response == 0) {
                        Iterable<String> purchaseDataList = ownedItems.getStringArrayList("INAPP_PURCHASE_DATA_LIST");

                        if (purchaseDataList != null) {
                            for (String purchaseData : purchaseDataList) {
                                JSONObject json = new JSONObject(purchaseData);
                                mService.consumePurchase(3, getPackageName(), json.getString("purchaseToken"));
                            }
                        }
                    }
                } catch (RemoteException | JSONException ignored) {
                    Toast.makeText(MainActivity.this, getString(R.string.exception), Toast.LENGTH_LONG).show();
                }*/
            }
        }

        private class ConsumePurchaseRunnable : Java.Lang.Object, Java.Lang.IRunnable
        {
            private Intent mData;

            public ConsumePurchaseRunnable(Intent data)
            {
                mData = data;
            }

            //@Override
            public void Run()
            {
                /*try {
                    JSONObject json = new JSONObject(mData.getStringExtra("INAPP_PURCHASE_DATA"));
                    mService.consumePurchase(3, getPackageName(), json.getString("purchaseToken"));
                } catch (JSONException | RemoteException ignored) {
                    Toast.makeText(MainActivity.this, getString(R.string.exception), Toast.LENGTH_LONG).show();
                }*/
            }
        }
    }
}