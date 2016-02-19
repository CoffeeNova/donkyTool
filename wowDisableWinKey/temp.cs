//private void BindFunction(BindParametrs bParam)
//        {           
//            #region CapsSwitch hook method
//            if (bParam.Execute == true && bParam.Method == bParam.BindMethod.Hook && bParam.PtrHookKey == IntPtr.Zero)
//            {
//                ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
//                //ProcessModule objExplorerModule = Process.GetProcessesByName("explorer")[0].MainModule;
//                //modifier
//                modifierKeyboardProcess = new LowLevelKeyboardProc((int nCode, IntPtr wp, IntPtr lp) =>
//                {
//                    if (nCode >= 0 && (Int32)wp == Const.WM_KEYDOWN)
//                    {               
//                        KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));
//                        if (objKeyInfo.key == Keys.LShiftKey || objKeyInfo.key == Keys.RShiftKey || Keys.LControlKey || objKeyInfo.key == Keys.RControlKey ||
//                            Keys.LAltKey || objKeyInfo.key == Keys.RAltKey)
//                        bParam.ModifierRegister(objKeyInfo.key);

//                        //    bParam.shiftDetected = true;
//                        //if (objKeyInfo.key == Keys.LControlKey || objKeyInfo.key == Keys.RControlKey)
//                        //    bParam.controlDetected = true;
//                        //if (objKeyInfo.key == Keys.LAltKey || objKeyInfo.key == Keys.RAltKey)
//                        //    bParam.altDetected = true;
//                    }
//                    if (nCode >= 0 && (Int32)wp == Const.WM_KEYUP)
//                    {
//                        KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));
//                        if (objKeyInfo.key == Keys.LShiftKey || objKeyInfo.key == Keys.RShiftKey || Keys.LControlKey || objKeyInfo.key == Keys.RControlKey ||
//                            Keys.LAltKey || objKeyInfo.key == Keys.RAltKey)
//                        bParam.ModifierKeyUnregister(objKeyInfo.key);

//                        //    bParam.ShiftDetected = false;
//                        //if (objKeyInfo.key == Keys.LControlKey || objKeyInfo.key == Keys.RControlKey)
//                        //    bParam.ControlDetected = false;
//                        //if (objKeyInfo.key == Keys.LAltKey || objKeyInfo.key == Keys.RAltKey)
//                        //    bParam.AltDetected = false;
//                    }
//                    return CallNextHookEx(ptrHookShiftKey, nCode, wp, lp);
//                });
//                bParam.ptrHookModKey = SetWindowsHookEx(13, modifierKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
                
//                //key
//                keyKeyboardProcess = new LowLevelKeyboardProc((int nCode, IntPtr wp, IntPtr lp) =>
//                {
//                    if (nCode >= 0 && (Int32)wp == Const.WM_KEYDOWN)
//                    {
//                        KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));
//                        if (objKeyInfo.key == bParam.Key && bParam.ShiftDetected && bParam.ModKey == bParam.ModifierKey.Shift)
//                        {
//                            bParam.ShiftModifierMethod();
//                            return (IntPtr)1;
//                        }
//                        else  if (objKeyInfo.key == bParam.key && bParam.controlDetected && bParam.ModKey == bParam.ModifierKey.Control)
//                        {
//                            bParam.ControlModifierMethod();
//                            return (IntPtr)1;
//                        }
//                        else  if (objKeyInfo.key == bParam.key && bParam.altDetected && bParam.ModKey == bParam.ModifierKey.Alt)
//                        {
//                            bParam.AltModifierMethod();
//                            return (IntPtr)1;
//                        }
//                        else if (objKeyInfo.key == bParam.key && !bParam.shiftDetected && !bParam.controlDetected && !bParam.altDetected)
//                        {
//                            bParam.WithoutModifierMethod();
//                            return (IntPtr)1;
//                        }
//                    }
//                    return CallNextHookEx(bParam.ptrHookKey, nCode, wp, lp);
//                });
//                bParam.ptrHookKey = SetWindowsHookEx(13, keyKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
//            }
//            else if (bParam.Execute == "false" || bParam.Method != bParam.BindMethod.Hook)
//            {
//                if (bParam.ptrHookKey != IntPtr.Zero)
//                {
//                    UnhookWindowsHookEx(bParam.ptrHookKey);
//                    bParam.ptrHookKey = IntPtr.Zero;
//                }
//                if (bParam.ptrHookModKey != IntPtr.Zero)
//                {
//                    UnhookWindowsHookEx(bParam.ptrHookModKey);
//                    bParam.ptrHookModKey = IntPtr.Zero;
//                }
//            }
//            #endregion
//            #region CapsSwitch RegisterHotKey method

//            if (bParam.execute == "true" && bParam.method == bParam.BindMethod.RegisterHotKey && !bParam.hotKeyRegistred)
//            {
//                bParam.hotKeyRegistred = RegisterHotKey(this.Handle, Const.SHIFTCAPS_REGISTER_ID, (int)KeyModifier.Shift, Keys.CapsLock.GetHashCode());
//                //ctrlCapsRegistred = RegisterHotKey(this.Handle, _CTRLCAPS_REGISTER_ID, (int)KeyModifier.Control, Keys.CapsLock.GetHashCode());
//                capsRegistred = RegisterHotKey(this.Handle, Const.CAPS_REGISTER_ID, (int)KeyModifier.None, Keys.CapsLock.GetHashCode());
//            }
//            else if (disableAll == "false" && capsSwitchEnable == "false" && !Const.use_Hook_Not_RegisterHotKey)
//            {
//                if (capsRegistred)
//                    UnregisterHotKey(this.Handle, Const.CAPS_REGISTER_ID);
//                if (shiftCapsRegistred)
//                    UnregisterHotKey(this.Handle, Const.SHIFTCAPS_REGISTER_ID);
//                //if(ctrlCapsRegistred)
//                //UnregisterHotKey(this.Handle, _CTRLCAPS_REGISTER_ID);
//            }
//            #endregion

//        }