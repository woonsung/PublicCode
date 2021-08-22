using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Media.Imaging;

namespace PublicCommonControls.WButton
{
    [ToolboxBitmap(typeof(Button))]
    [ToolboxItem(true)]
    public class WButton : Control, IDisposable
    {
        #region Private Memebers
        bool _scrollText;
        bool _drawBorder;
        bool _drawBorderOnDisabled;
        bool _centerImage;
        bool _pushed;
        bool _isAnimating;
        bool _mouseDown;
        bool _leftDown;
        bool _rightDown;
        bool _enabled;
        bool _absoluteSubTextOffset;
        bool _shiftText;
        bool _subTextVisible;
        bool _textOnImage;
        bool _rightPushed;
        bool _leftPushed;
        bool _autoSizeFont;
        bool _fitImage;
        bool _fixedRatio;
        int _radius;
        int _decimalPlaces;
        int _numericCount;
        int _alpha;
        int _underLineThick;
        int _pushedUnderLineThick;
        int _scrollingValue;
        int _scrollingOffset;
        int _scrollingInterval;
        int _accelationInterval;
        int _accelationIntervalHighSpeed;
        float _borderThick;
        float _checkBoxThick;
        float _imagePercentage;
        float _outerTextThick;
        float _outerSubTextThick;
        float _flowTextLengthOffset;
        double _value;
        double _previousValue;
        double _maxValue;
        double _minValue;
        double _increasment;
        double _increasmentOnRightClick;
        string _subText;
        Image _image;
        Image _pushedImage;
        Image _disabledImage;
        EnButtonColor _buttonColor;
        EnButtonColor _pushedColor;
        EnButtonColor _mouseDownColor;
        EnButtonColor _arrowColor;
        EnButtonColor _pushedArrowColor;
        EnButtonType _buttonType;
        EnButtonColor _checkerBoxColor;
        EnArrowDirection _arrowDirection;
        EnUnderLine _underLine;
        EnShadow _shadow;
        ContentAlignment _textAlignment;
        ContentAlignment _subTextAlignment;
        Color _checkerBoxSolidColor;
        Color _buttonSolidColor;
        Color _disabledSolidColor;
        Color _innerForeColor;
        Color _outerForeColor;
        Color _disabledInnerForeColor;
        Color _disabledOuterForeColor;
        Color _pushedInnerForeColor;
        Color _pushedOuterForeColor;
        Color _borderColor;
        Color _innerSubTextColor;
        Color _outerSubTextColor;
        Color _pushedInnerSubTextColor;
        Color _pushedOuterSubTextColor;
        Color _disabledInnerSubTextColor;
        Color _disabledOuterSubTextColor;
        Color _arrowSolidColor;
        Color _pushedArrowSolidColor;
        Color _disabledArrowColor;
        Color _pushedSolidColor;
        Color _mouseDownSolidColor;
        Color _underLineColor;
        Color _pushedUnderLineColor;
        Color _disabledUnderLineColor;
        Color _underLineMouseOver;
        Color _mouseOverInnerForeColor;
        Color _mouseOverOuterForeColor;
        Font _subTextFont;
        Padding _textPadding;
        Padding _subTextPadding;
        Padding _imagePadding;
        Timer _timerAccelation;
        Timer _scrollTimer;
        #endregion
        #region Events
        [Category("Property Changed")]
        public event EventHandler RadiusChanged;
        protected virtual void OnRadiusChanged(EventArgs e)
        {
            EventHandler handler = this.RadiusChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler FitImageChanged;
        protected virtual void OnFitImageChanged(EventArgs e)
        {
            EventHandler handler = this.FitImageChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler ArrowDirectionChanged;
        protected virtual void OnArrowDirectionChanged(EventArgs e)
        {
            EventHandler handler = this.ArrowDirectionChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler DecimalPlacesChanged;
        protected virtual void OnDecimalPlacesChanged(EventArgs e)
        {
            EventHandler handler = this.DecimalPlacesChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }

        [Category("Property Changed")]
        public event EventHandler ButtonColorChanged;
        protected virtual void OnButtonColorChanged(EventArgs e)
        {
            EventHandler handler = this.ButtonColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler CheckerBoxColorChanged;
        protected virtual void OnCheckerBoxColorChanged(EventArgs e)
        {
            EventHandler handler = this.CheckerBoxColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler CheckerBoxSolidColorChanged;
        protected virtual void OnCheckerBoxSolidColorChanged(EventArgs e)
        {
            EventHandler handler = this.CheckerBoxSolidColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }

        [Category("Property Changed")]
        public event EventHandler ButtonSolidColorChanged;
        protected virtual void OnButtonSolidColorChanged(EventArgs e)
        {
            EventHandler handler = this.ButtonSolidColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler ImageChanged;
        protected virtual void OnImageChanged(EventArgs e)
        {
            EventHandler handler = this.ImageChanged;
            this._isAnimating = false;
            if (this._image != null)
            {
                try
                {
                    this._isAnimating = ImageAnimator.CanAnimate(this._image);
                    if (this._isAnimating) // && (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime))
                        ImageAnimator.Animate(this._image, this.OnFrameChangedHandler);
                }
                catch (ArgumentException)
                { }
            }
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public new event EventHandler EnabledChanged;
        protected new virtual void OnEnabledChanged(EventArgs e)
        {
            EventHandler handler = this.EnabledChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler FixedRatioChanged;
        protected virtual void OnFixedRatioChanged(EventArgs e)
        {
            EventHandler handler = this.FixedRatioChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public new event EventHandler ForeColorChanged;
        protected new virtual void OnForeColorChanged(EventArgs e)
        {
            EventHandler handler = this.ForeColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler SubTextChanged;
        protected virtual void OnSubTextChanged(EventArgs e)
        {
            EventHandler handler = this.SubTextChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler SubTextFontChanged;
        protected virtual void OnSubTextFontChanged(EventArgs e)
        {
            EventHandler handler = this.SubTextFontChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler BorderChanged;
        protected virtual void OnBorderChanged(EventArgs e)
        {
            EventHandler handler = this.BorderChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler DrawBorderOnDiabledChanged;
        protected virtual void OnDrawBorderOnDiabledChanged(EventArgs e)
        {
            EventHandler handler = this.DrawBorderOnDiabledChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler SubTextVisibleChanged;
        protected virtual void OnSubTextVisibleChanged(EventArgs e)
        {
            EventHandler handler = this.SubTextVisibleChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler AutoSizeFontChanged;
        protected virtual void OnAutoSizeFontChanged(EventArgs e)
        {
            EventHandler handler = this.AutoSizeFontChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler ShadowChanged;
        protected virtual void OnShadowChanged(EventArgs e)
        {
            EventHandler handler = this.ShadowChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        public event EventHandler AlphaChanged;
        protected virtual void OnAlphaChanged(EventArgs e)
        {
            EventHandler handler = this.AlphaChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }

        [Category("Property Changed")]
        public event EventHandler TextAlignmentChanged;
        protected virtual void OnTextAlignmentChanged(EventArgs e)
        {
            EventHandler handler = this.TextAlignmentChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler SubTextAlignmentChanged;
        protected virtual void OnSubTextAlignmentChanged(EventArgs e)
        {
            EventHandler handler = this.SubTextAlignmentChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler TextPaddingChanged;
        protected virtual void OnTextPaddingChanged(EventArgs e)
        {
            EventHandler handler = this.TextPaddingChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler SubTextPaddingChanged;
        protected virtual void OnSubTextPaddingChanged(EventArgs e)
        {
            EventHandler handler = this.SubTextPaddingChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler ImagePaddingChanged;
        protected virtual void OnImagePaddingChanged(EventArgs e)
        {
            EventHandler handler = this.ImagePaddingChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedImageChanged;
        protected virtual void OnPushedImageChanged(EventArgs e)
        {
            EventHandler handler = this.PushedImageChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler DisabledImageChanged;
        protected virtual void OnDisabledImageChanged(EventArgs e)
        {
            EventHandler handler = this.DisabledImageChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler UnderLineColorChanged;
        protected virtual void OnUnderLineColorChanged(EventArgs e)
        {
            EventHandler handler = this.UnderLineColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler UnderLineMouseOverColorChanged;
        protected virtual void OnUnderLineMouseOverColorChanged(EventArgs e)
        {
            EventHandler handler = this.UnderLineMouseOverColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedUnderLineColorChanged;
        protected virtual void OnPushedUnderLineColorChanged(EventArgs e)
        {
            EventHandler handler = this.PushedUnderLineColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler UnderLineChanged;
        protected virtual void OnUnderLineChanged(EventArgs e)
        {
            EventHandler handler = this.UnderLineChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler UnderLineThickChanged;
        protected virtual void OnUnderLineThickChanged(EventArgs e)
        {
            EventHandler handler = this.UnderLineThickChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedUnderLineThickChanged;
        protected virtual void OnPushedUnderLineThickChanged(EventArgs e)
        {
            EventHandler handler = this.PushedUnderLineThickChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler ShiftTextChanged;
        protected virtual void OnShiftTextChanged(EventArgs e)
        {
            EventHandler handler = this.ShiftTextChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler AbsoultSubTextOffsetChanged;
        protected virtual void OnAbsoultSubTextOffsetChanged(EventArgs e)
        {
            EventHandler handler = this.AbsoultSubTextOffsetChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler TextOnImageChanged;
        protected virtual void OnTextOnImageChanged(EventArgs e)
        {
            EventHandler handler = this.TextOnImageChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler ArrowColorChanged;
        protected virtual void OnArrowColorChanged(EventArgs e)
        {
            EventHandler handler = this.ArrowColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedArrowColorChanged;
        protected virtual void OnPushedArrowColorChanged(EventArgs e)
        {
            EventHandler handler = this.PushedArrowColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler ArrowSolidColorChanged;
        protected virtual void OnArrowSolidColorChanged(EventArgs e)
        {
            EventHandler handler = this.ArrowSolidColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedArrowSolidColorChanged;
        protected virtual void OnPushedArrowSolidColorChanged(EventArgs e)
        {
            EventHandler handler = this.PushedArrowSolidColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler DisabledArrowColorChanged;
        protected virtual void OnDisabledArrowColorChanged(EventArgs e)
        {
            EventHandler handler = this.DisabledArrowColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler MouseDownColorChanged;
        protected virtual void OnMouseDownColorChanged(EventArgs e)
        {
            EventHandler handler = this.MouseDownColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }

        [Category("Property Changed")]
        public event EventHandler OuterTextThickChanged;
        protected virtual void OnOuterTextThickChanged(EventArgs e)
        {
            EventHandler handler = this.OuterTextThickChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }

        [Category("Property Changed")]
        public event EventHandler FlowTextLengthOffsetChanged;
        protected virtual void OnFlowTextLengthOffsetChanged(EventArgs e)
        {
            EventHandler handler = this.FlowTextLengthOffsetChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler OuterSubTextThickChanged;
        protected virtual void OnOuterSubTextThickChanged(EventArgs e)
        {
            EventHandler handler = this.OuterSubTextThickChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler InnerForeColorChanged;
        protected virtual void OnInnerForeColorChanged(EventArgs e)
        {
            EventHandler handler = this.InnerForeColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler OuterForeColorChanged;
        protected virtual void OnOuterForeColorChanged(EventArgs e)
        {
            EventHandler handler = this.OuterForeColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedInnerForeColorChanged;
        protected virtual void OnPushedInnerForeColorChanged(EventArgs e)
        {
            EventHandler handler = this.PushedInnerForeColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedOuterForeColorChanged;
        protected virtual void OnPushedOuterForeColorChanged(EventArgs e)
        {
            EventHandler handler = this.PushedOuterForeColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler DisabledInnerForeColorChanged;
        protected virtual void OnDisabledInnerForeColorChanged(EventArgs e)
        {
            EventHandler handler = this.DisabledInnerForeColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler DisabledOuterForeColorChanged;
        protected virtual void OnDisabledOuterForeColorChanged(EventArgs e)
        {
            EventHandler handler = this.DisabledOuterForeColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler InnerSubTextColorChanged;
        protected virtual void OnInnerSubTextColorChanged(EventArgs e)
        {
            EventHandler handler = this.InnerSubTextColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler OuterSubTextColorChanged;
        protected virtual void OnOuterSubTextColorChanged(EventArgs e)
        {
            EventHandler handler = this.OuterSubTextColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedInnerSubTextColorChanged;
        protected virtual void OnPushedInnerSubTextColorChanged(EventArgs e)
        {
            EventHandler handler = this.PushedInnerSubTextColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedOuterSubTextColorChanged;
        protected virtual void OnPushedOuterSubTextColorChanged(EventArgs e)
        {
            EventHandler handler = this.PushedOuterSubTextColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler DisabledInnerSubTextColorChanged;
        protected virtual void OnDisabledInnerSubTextColorChanged(EventArgs e)
        {
            EventHandler handler = this.DisabledInnerSubTextColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler DisabledOuterSubTextColorChanged;
        protected virtual void OnDisabledOuterSubTextColorChanged(EventArgs e)
        {
            EventHandler handler = this.DisabledOuterSubTextColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler DrawBorderChanged;
        protected virtual void OnDrawBorderChanged(EventArgs e)
        {
            EventHandler handler = this.DrawBorderChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler CenterImageChanged;
        protected virtual void OnCenterImageChanged(EventArgs e)
        {
            EventHandler handler = this.CenterImageChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler BorderColorChanged;
        protected virtual void OnBorderColorChanged(EventArgs e)
        {
            EventHandler handler = this.BorderColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler BorderThickChanged;
        protected virtual void OnBorderThickChanged(EventArgs e)
        {
            EventHandler handler = this.BorderThickChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler ImagePercentageChanged;
        protected virtual void OnImagePercentageChanged(EventArgs e)
        {
            EventHandler handler = this.ImagePercentageChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler CheckerBoxThickChanged;
        protected virtual void OnCheckerBoxThickChangedChanged(EventArgs e)
        {
            EventHandler handler = this.CheckerBoxThickChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedColorChanged;
        protected virtual void OnPushedColorChanged(EventArgs e)
        {
            EventHandler handler = this.PushedColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler PushedSolidColorChanged;
        protected virtual void OnPushedSolidColorChanged(EventArgs e)
        {
            EventHandler handler = this.PushedSolidColorChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        [Category("Property Changed")]
        public event EventHandler ButtonTypeChanged;
        protected virtual void OnButtonTypeChanged(EventArgs e)
        {
            EventHandler handler = this.ButtonTypeChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }


        [Category("Action")]
        public new event EventHandler<ButtonClickEventArgs> Click;
        protected virtual void OnClicked(ButtonClickEventArgs e)
        {
            EventHandler<ButtonClickEventArgs> handler = this.Click;
            if (handler != null)
                handler(this, e);
        }
        [Category("Action")]
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs e)
        {
            EventHandler handler = this.ValueChanged;
            this.Invalidate();
            if (handler != null)
                handler(this, e);
        }
        #endregion
        #region Constructor
        public WButton()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.StandardDoubleClick, false);
            _shadow = EnShadow.None;
            _scrollText = false;
            _drawBorder = true;
            _drawBorderOnDisabled = true;
            _centerImage = false;
            _pushed = false;
            _mouseDown = false;
            _enabled = true;
            _absoluteSubTextOffset = true;
            _shiftText = false;
            _subTextVisible = false;
            _textOnImage = false;
            _rightPushed = false;
            _leftPushed = false;
            _autoSizeFont = false;
            _fitImage = false;
            _fixedRatio = false;
            _radius = 10;
            _decimalPlaces = 2;
            _numericCount = 0;
            _alpha = 255;
            _underLineThick = 3;
            _pushedUnderLineThick = 3;
            _scrollingValue = 0;
            _scrollingOffset = 5;
            _scrollingInterval = 300;
            _borderThick = 1f;
            _imagePercentage = 1f;
            _checkBoxThick = 1f;
            _outerTextThick = 1f;
            _outerSubTextThick = 1f;
            _flowTextLengthOffset = 1f;
            _value = 0;
            _maxValue = 100;
            _minValue = 0;
            _increasment = 1;
            _increasmentOnRightClick = 10;
            _subText = string.Empty;
            _image = null;
            _pushedImage = null;
            _disabledImage = null;
            _buttonColor = EnButtonColor.GrayGradient1;
            _checkerBoxColor = EnButtonColor.WhiteGradient;
            _pushedColor = EnButtonColor.Solid;
            _mouseDownColor = EnButtonColor.BlueGradient;
            _arrowColor = EnButtonColor.GrayGradient2;
            _pushedArrowColor = EnButtonColor.Solid;
            _buttonType = EnButtonType.NormalButton;
            _arrowDirection = EnArrowDirection.LeftRight;
            _underLine = EnUnderLine.None;
            _textAlignment = ContentAlignment.MiddleCenter;
            _subTextAlignment = ContentAlignment.MiddleCenter;
            _checkerBoxSolidColor = Color.White;
            _disabledSolidColor = Color.White;
            _innerForeColor = Color.White;
            _outerForeColor = Color.White;
            _disabledInnerForeColor = Color.FromArgb(200, 200, 200);
            _disabledOuterForeColor = Color.FromArgb(200, 200, 200);
            _borderColor = Color.FromArgb(180, 180, 180);
            _innerSubTextColor = Color.Black;
            _outerSubTextColor = Color.Black;
            _pushedInnerSubTextColor = Color.White;
            _pushedOuterSubTextColor = Color.White;
            _disabledInnerSubTextColor = Color.FromArgb(200, 200, 200);
            _disabledOuterSubTextColor = Color.FromArgb(200, 200, 200);
            _arrowSolidColor = Color.FromArgb(200, 200, 200); ;
            _pushedArrowSolidColor = Color.White;
            _disabledArrowColor = Color.FromArgb(200, 200, 200);
            _pushedInnerForeColor = Color.Black;
            _pushedOuterForeColor = Color.Black;
            _pushedSolidColor = Color.White;
            _mouseDownSolidColor = Color.Blue;
            _underLineColor = Color.FromArgb(100, 100, 100);
            _pushedUnderLineColor = Color.FromArgb(0, 255, 0);
            _disabledUnderLineColor = Color.Black;
            _underLineMouseOver = Color.Empty;
            _mouseOverInnerForeColor = Color.Empty;
            _mouseOverOuterForeColor = Color.Empty;
            _subTextFont = DefaultFont;
            _textPadding = new Padding(0, 0, 0, 0);
            _subTextPadding = new Padding(0, 0, 0, 0); ;
            _imagePadding = new Padding(0, 0, 0, 0); ;
            _timerAccelation = new Timer();
            _timerAccelation.Tick += _timerAccelation_Tick;
            _accelationInterval = 400;
            _accelationIntervalHighSpeed = 50;
            _timerAccelation.Interval = _accelationInterval;

        }

        private void _timerAccelation_Tick(object sender, EventArgs e)
        {
            if (_numericCount == 0)
                _timerAccelation.Interval = 600;
            else if (_numericCount > 3)
                _timerAccelation.Interval = 50;
            if (_rightPushed)
            {
                if (_leftDown)
                    _value += _increasment;
                else if (_rightDown)
                    _value += _increasmentOnRightClick;
                if (_value > _maxValue)
                {
                    _value = _maxValue;
                    _timerAccelation.Stop();
                    _timerAccelation.Interval = 600;
                    _numericCount = 0;
                }
                else
                    _numericCount++;
            }
            else if (_leftPushed)
            {
                if (_leftDown)
                    _value -= _increasment;
                else if (_rightDown)
                    _value -= _increasmentOnRightClick;
                if (_value < _minValue)
                {
                    _value = _minValue;
                    _timerAccelation.Stop();
                    _timerAccelation.Interval = 600;
                    _numericCount = 0;
                }
                else
                    _numericCount++;
            }

            this.Invalidate();
        }
        #endregion
        #region Properties
        [Category("Appearance")]
        [DefaultValue(20)]
        public int Radius
        {
            get { return _radius; }
            set
            {
                if (_radius != value)
                {
                    _radius = value;
                    OnRadiusChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool FitImage
        {
            get { return _fitImage; }
            set
            {
                if (_fitImage != value)
                {
                    _fitImage = value;
                    OnFitImageChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnArrowDirection), "LeftRight")]
        public EnArrowDirection ArrowDirection
        {
            get { return _arrowDirection; }
            set
            {
                if (_arrowDirection != value)
                {
                    _arrowDirection = value;
                    OnArrowDirectionChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(2)]
        public int DecimalPlaces
        {
            get { return _decimalPlaces; }
            set
            {
                if (_decimalPlaces != value)
                {
                    _decimalPlaces = value;
                    this.Text = Value.ToString();
                    OnDecimalPlacesChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(0)]
        public double Value
        {
            get { return _value; }
            set
            {
                if (this._buttonType != EnButtonType.NumericUpDown)
                    return;
                if (_value != value)
                {
                    if (value >= _maxValue)
                        _value = _maxValue;
                    else if (value <= _minValue)
                        _value = _minValue;
                    else
                        _value = value;
                    OnValueChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        public Font SubTextFont
        {
            get { return _subTextFont; }
            set
            {
                if (_subTextFont != value)
                {
                    _subTextFont = value;
                    OnSubTextFontChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool DrawBorderOnDisabled
        {
            get { return _drawBorderOnDisabled; }
            set
            {
                if (_drawBorderOnDisabled != value)
                {
                    _drawBorder = value;
                    OnDrawBorderOnDiabledChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool SubTextVisible
        {
            get { return _subTextVisible; }
            set
            {
                if (_subTextVisible != value)
                {
                    _subTextVisible = value;
                    OnSubTextVisibleChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool AutoSizeFont
        {
            get { return _autoSizeFont; }
            set
            {
                if (_autoSizeFont != value)
                {
                    _autoSizeFont = value;
                    OnAutoSizeFontChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnShadow), "None")]
        public EnShadow Shadow
        {
            get { return _shadow; }
            set
            {
                if (_shadow != value)
                {
                    _shadow = value;
                    OnShadowChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ScrollText
        {
            get { return _scrollText; }
            set
            {
                if (_scrollText != value)
                {
                    _scrollText = value;
                    if (_scrollText)
                    {
                        if (_scrollTimer == null)
                        {
                            _scrollTimer = new Timer();
                            _scrollTimer.Interval = _scrollingInterval;
                            _scrollTimer.Tick += _scrollTimer_Tick;
                            _scrollTimer.Start();
                        }
                    }
                    else
                    {
                        _scrollTimer.Stop();
                        _scrollTimer.Dispose();
                        _scrollTimer = null;
                    }
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public int Alpha
        {
            get { return _alpha; }
            set
            {
                if (_alpha != value)
                {
                    _alpha = value;
                    OnAlphaChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        public ContentAlignment TextAlignment
        {
            get { return _textAlignment; }
            set
            {
                if (_textAlignment != value)
                {
                    _textAlignment = value;
                    OnTextAlignmentChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        public ContentAlignment SubTextAlignment
        {
            get { return _subTextAlignment; }
            set
            {
                if (_subTextAlignment != value)
                {
                    _subTextAlignment = value;
                    OnTextAlignmentChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(null)]
        public Image Image
        {
            get { return _image; }
            set
            {
                if (_image != null)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            if (this._isAnimating)
                                ImageAnimator.StopAnimate(this.Image, this.OnFrameChangedHandler);
                        }));
                    }
                    else
                        ImageAnimator.StopAnimate(this.Image, this.OnFrameChangedHandler);
                }
                Image tmp = null;
                if (_image != null)
                    tmp = _image;
                if (ImageAnimator.CanAnimate(value))
                {

                    using (MemoryStream ms = new MemoryStream())
                    {
                        value.Save(ms, value.RawFormat);
                        byte[] bytes = ms.ToArray();
                        _image = Image.FromStream(new MemoryStream(bytes));
                    }
                }
                else
                {
                    _image = value;
                }
                if (tmp != null)
                    tmp.Dispose();
                OnImageChanged(EventArgs.Empty);
            }
        }
        [Category("Appearance")]
        [DefaultValue(null)]
        public Image PushedImage
        {
            get { return _pushedImage; }
            set
            {
                if (_pushedImage != value)
                {
                    if (_pushedImage != value)
                    {
                        if (_pushedImage != null)
                        {
                            _pushedImage.Dispose();
                            _pushedImage = null;
                        }
                        _pushedImage = value;
                        OnPushedImageChanged(EventArgs.Empty);
                    }
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(null)]
        public Image DisabledImage
        {
            get { return _disabledImage; }
            set
            {
                if (_disabledImage != value)
                {
                    if (_disabledImage != value)
                    {
                        if (_disabledImage != null)
                        {
                            _disabledImage.Dispose();
                            _disabledImage = null;
                        }
                        _disabledImage = value;
                        OnDisabledImageChanged(EventArgs.Empty);
                    }
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "100,100,100")]
        public Color UnderLineColor
        {
            get { return _underLineColor; }
            set
            {
                if (_underLineColor != value)
                {
                    _underLineColor = value;
                    OnUnderLineColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "100,100,100")]
        public Color UnderLineMouseOverColor
        {
            get { return _underLineMouseOver; }
            set
            {
                if (_underLineMouseOver != value)
                {
                    _underLineMouseOver = value;
                    OnUnderLineMouseOverColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "0,255,0")]
        public Color PushedUnderLineColor
        {
            get { return _pushedUnderLineColor; }
            set
            {
                if (_pushedUnderLineColor != value)
                {
                    _pushedUnderLineColor = value;
                    OnPushedUnderLineColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnUnderLine), "None")]
        public EnUnderLine UnderLine
        {
            get { return _underLine; }
            set
            {
                if (_underLine != value)
                {
                    _underLine = value;
                    OnUnderLineChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(3)]
        public int UnderLineThick
        {
            get { return _underLineThick; }
            set
            {
                if (_underLineThick != value)
                {
                    _underLineThick = value;
                    OnUnderLineThickChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(3)]
        public int PushedUnderLineThick
        {
            get { return _pushedUnderLineThick; }
            set
            {
                if (_pushedUnderLineThick != value)
                {
                    _pushedUnderLineThick = value;
                    OnPushedUnderLineThickChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue("")]
        public string SubText
        {
            get { return _subText; }
            set
            {
                if (_subText != value)
                {
                    _subText = value;
                    OnSubTextChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShiftTexts
        {
            get { return _shiftText; }
            set
            {
                if (_shiftText != value)
                {
                    _shiftText = value;
                    OnShiftTextChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool AbsoluteSubTextOffset
        {
            get { return _absoluteSubTextOffset; }
            set
            {
                if (_absoluteSubTextOffset != value)
                {
                    _absoluteSubTextOffset = value;
                    OnAbsoultSubTextOffsetChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool TextTopOfImage
        {
            get { return _textOnImage; }
            set
            {
                if (_textOnImage != value)
                {
                    _textOnImage = value;
                    OnTextOnImageChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnButtonColor), "Gradient1")]
        public EnButtonColor ButtonColor
        {
            get { return _buttonColor; }
            set
            {
                if (_buttonColor != value)
                {
                    _buttonColor = value;
                    OnButtonColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnButtonColor), "WhiteGradient")]
        public EnButtonColor CheckerBoxColor
        {
            get { return _checkerBoxColor; }
            set
            {
                if (_checkerBoxColor != value)
                {
                    _checkerBoxColor = value;
                    OnCheckerBoxColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color CheckerBoxSolidColor
        {
            get { return _checkerBoxSolidColor; }
            set
            {
                if (_checkerBoxSolidColor != value)
                {
                    _checkerBoxSolidColor = value;
                    OnCheckerBoxSolidColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "180,180,180")]
        public Color ButtonSolidColor
        {
            get { return _buttonSolidColor; }
            set
            {
                if (_buttonSolidColor != value)
                {
                    _buttonSolidColor = value;
                    OnButtonSolidColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnButtonColor), "Gradient1")]
        public EnButtonColor ArrowColor
        {
            get { return _arrowColor; }
            set
            {
                if (_arrowColor != value)
                {
                    _arrowColor = value;
                    OnArrowColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnButtonColor), "Solid")]
        public EnButtonColor PushedArrowColor
        {
            get { return _pushedArrowColor; }
            set
            {
                if (_pushedArrowColor != value)
                {
                    _pushedArrowColor = value;
                    OnPushedArrowColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "200,200,200")]
        public Color ArrowSolidColor
        {
            get { return _arrowSolidColor; }
            set
            {
                if (_arrowSolidColor != value)
                {
                    _arrowSolidColor = value;
                    OnArrowSolidColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color PushedArrowSolidColor
        {
            get { return _pushedArrowSolidColor; }
            set
            {
                if (_pushedArrowSolidColor != value)
                {
                    _pushedArrowSolidColor = value;
                    OnPushedArrowSolidColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "200, 200, 200")]
        public Color DiabledArrowColor
        {
            get { return _disabledArrowColor; }
            set
            {
                if (_disabledArrowColor != value)
                {
                    _disabledArrowColor = value;
                    OnDisabledArrowColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnButtonColor), "GradientBlue")]
        public EnButtonColor MouseDownColor
        {
            get { return _mouseDownColor; }
            set
            {
                if (_mouseDownColor != value)
                {
                    _mouseDownColor = value;
                    OnMouseDownColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(1f)]
        public float OuterTextThick
        {
            get { return _outerTextThick; }
            set
            {
                if (_outerTextThick != value)
                {
                    _outerTextThick = value;
                    OnOuterTextThickChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(1f)]
        public float OuterSubTextThick
        {
            get { return _outerSubTextThick; }
            set
            {
                if (_outerSubTextThick != value)
                {
                    _outerSubTextThick = value;
                    OnOuterTextThickChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(1f)]
        public float FlowTextLengthOffset
        {
            get { return _flowTextLengthOffset; }
            set
            {
                if (_flowTextLengthOffset != value)
                {
                    if (_flowTextLengthOffset <= 0)
                        _flowTextLengthOffset = 1;
                    _flowTextLengthOffset = value;
                    OnFlowTextLengthOffsetChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color InnerForeColor
        {
            get { return _innerForeColor; }
            set
            {
                if (_innerForeColor != value)
                {
                    _innerForeColor = value;
                    OnInnerForeColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color OuterForeColor
        {
            get { return _outerForeColor; }
            set
            {
                if (_outerForeColor != value)
                {
                    _outerForeColor = value;
                    OnOuterForeColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color PushedInnerForeColor
        {
            get { return _pushedInnerForeColor; }
            set
            {
                if (_pushedInnerForeColor != value)
                {
                    _pushedInnerForeColor = value;
                    OnPushedInnerForeColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color PushedOuterForeColor
        {
            get { return _pushedOuterForeColor; }
            set
            {
                if (_pushedOuterForeColor != value)
                {
                    _pushedOuterForeColor = value;
                    OnPushedInnerForeColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "200,200,200")]
        public Color DisabledInnerForeColor
        {
            get { return _disabledInnerForeColor; }
            set
            {
                if (_disabledInnerForeColor != value)
                {
                    _disabledInnerForeColor = value;
                    OnDisabledInnerForeColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "200,200,200")]
        public Color DisabledOuterForeColor
        {
            get { return _disabledOuterForeColor; }
            set
            {
                if (_disabledOuterForeColor != value)
                {
                    _disabledOuterForeColor = value;
                    OnDisabledOuterForeColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color InnerSubTextColor
        {
            get { return _innerSubTextColor; }
            set
            {
                if (_innerSubTextColor != value)
                {
                    _innerSubTextColor = value;
                    OnInnerSubTextColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color OuterSubTextColor
        {
            get { return _outerSubTextColor; }
            set
            {
                if (_outerSubTextColor != value)
                {
                    _outerSubTextColor = value;
                    OnOuterSubTextColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color PushedInnerSubTextColor
        {
            get { return _pushedInnerSubTextColor; }
            set
            {
                if (_pushedInnerSubTextColor != value)
                {
                    _pushedInnerSubTextColor = value;
                    OnPushedInnerSubTextColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color PushedOuterSubTextColor
        {
            get { return _pushedOuterSubTextColor; }
            set
            {
                if (_pushedOuterSubTextColor != value)
                {
                    _pushedOuterSubTextColor = value;
                    OnPushedOuterSubTextColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "200,200,200")]
        public Color DisabledInnerSubTextColor
        {
            get { return _disabledInnerSubTextColor; }
            set
            {
                if (_disabledInnerSubTextColor != value)
                {
                    _disabledInnerSubTextColor = value;
                    OnDisabledInnerSubTextColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "200,200,200")]
        public Color DisabledOuterSubTextColor
        {
            get { return _disabledOuterSubTextColor; }
            set
            {
                if (_disabledOuterSubTextColor != value)
                {
                    _disabledOuterSubTextColor = value;
                    OnDisabledOuterSubTextColorChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool DrawBorder
        {
            get { return _drawBorder; }
            set
            {
                if (_drawBorder != value)
                {
                    _drawBorder = value;
                    OnDrawBorderChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool CenterImage
        {
            get { return _centerImage; }
            set
            {
                if (_centerImage != value)
                {
                    _centerImage = value;
                    OnCenterImageChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "180, 180, 180")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    OnBorderColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(1f)]
        public float BorderThick
        {
            get { return _borderThick; }
            set
            {
                if (_borderThick != value)
                {
                    _borderThick = value;
                    OnBorderThickChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(1f)]
        public float ImagePercentage
        {
            get { return _imagePercentage; }
            set
            {
                if (_imagePercentage != value)
                {
                    _imagePercentage = value;
                    OnImagePercentageChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(1f)]
        public float CheckerBoxThick
        {
            get { return _checkBoxThick; }
            set
            {
                if (_checkBoxThick != value)
                {
                    _checkBoxThick = value;
                    OnCheckerBoxThickChangedChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnButtonColor), "White")]
        public EnButtonColor PushedColor
        {
            get { return _pushedColor; }
            set
            {
                if (_pushedColor != value)
                {
                    _pushedColor = value;
                    OnPushedColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnButtonColor), "White")]
        public Color PushedSolidColor
        {
            get { return _pushedSolidColor; }
            set
            {
                if (_pushedSolidColor != value)
                {
                    _pushedSolidColor = value;
                    OnPushedSolidColorChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(true)]
        public new bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnEnabledChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool FixedRatio
        {
            get { return _fixedRatio; }
            set
            {
                if (_fixedRatio != value)
                {
                    _fixedRatio = value;
                    OnFixedRatioChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnButtonType), "NormalButton")]
        public EnButtonType ButtonType
        {
            get { return _buttonType; }
            set
            {
                if (_buttonType != value)
                {
                    _buttonType = value;
                    if (_buttonType == EnButtonType.NumericUpDown)
                        this.Value = _minValue;
                    OnButtonTypeChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Appearance")]
        [DefaultValue(typeof(EnButtonType), "NormalButton")]
        public bool Pushed
        {
            get { return _pushed; }
            set
            {
                if (_pushed != value)
                {
                    string tmp;
                    if (string.IsNullOrEmpty(SubText))
                        tmp = this.Text;
                    else
                        tmp = _shiftText ? this.SubText : this.Text;
                    ButtonClickEventArgs args = new ButtonClickEventArgs(tmp, value);
                    switch (ButtonType)
                    {
                        case EnButtonType.NormalButton:
                        case EnButtonType.NumericUpDown:
                            OnClicked(new ButtonClickEventArgs(tmp, true));
                            _pushed = false;
                            break;
                        case EnButtonType.CheckBox:
                        case EnButtonType.RightCheckBox:
                        case EnButtonType.PushButton:
                        case EnButtonType.Label:
                            OnClicked(args);
                            if (!args.Cancel)
                                _pushed = value;
                            break;
                        case EnButtonType.RadioButton:
                            OnClicked(args);
                            if (!args.Cancel)
                            {
                                _pushed = value;
                                ProcessRadioButton();
                            }
                            break;
                    }
                }
                this.Invalidate();
            }
        }
        [Category("Layout")]
        [DefaultValue(typeof(Padding), "0,0,0,0")]
        public Padding TextPadding
        {
            get { return _textPadding; }
            set
            {
                if (_textPadding != value)
                {
                    _textPadding = value;
                    OnTextPaddingChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Layout")]
        [DefaultValue(typeof(Padding), "0,0,0,0")]
        public Padding SubTextPadding
        {
            get { return _subTextPadding; }
            set
            {
                if (_subTextPadding != value)
                {
                    _subTextPadding = value;
                    OnSubTextPaddingChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Layout")]
        [DefaultValue(typeof(Padding), "0,0,0,0")]
        public Padding ImagePadding
        {
            get { return _imagePadding; }
            set
            {
                if (_imagePadding != value)
                {
                    _imagePadding = value;
                    OnImagePaddingChanged(EventArgs.Empty);
                }
            }
        }
        [Category("Action")]
        [DefaultValue(5)]
        public int ScrollingOffset
        {
            get { return _scrollingOffset; }
            set
            {
                if (_scrollingOffset != value)
                {
                    _scrollingOffset = value;
                }
            }
        }
        [Category("Action")]
        [DefaultValue(300)]
        public int ScrollingInterval
        {
            get { return _scrollingInterval; }
            set
            {
                if (_scrollingInterval != value)
                {
                    _scrollingInterval = value;
                }
            }
        }
        [Category("Action")]
        [DefaultValue(0)]
        public double MinValue
        {
            get { return _minValue; }
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    if (this.Value <= _minValue)
                        this.Value = _minValue;
                }
            }
        }
        [Category("Action")]
        [DefaultValue(10000)]
        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    if (this.Value >= _maxValue)
                        this.Value = _maxValue;
                }
            }
        }
        [Category("Action")]
        [DefaultValue(1)]
        public double Increasement
        {
            get { return _increasment; }
            set
            {
                if (_increasment != value)
                {
                    _increasment = value;
                }
            }
        }
        [Category("Action")]
        [DefaultValue(10)]
        public double IncreasementOnRightClick
        {
            get { return _increasmentOnRightClick; }
            set
            {
                if (_increasmentOnRightClick != value)
                {
                    _increasmentOnRightClick = value;
                }
            }
        }

        [Category("Action")]
        [DefaultValue(400)]
        public int AccelationInterval
        {
            get { return _accelationInterval; }
            set
            {
                if (_accelationInterval != value)
                {
                    _accelationInterval = value;
                    if (_timerAccelation != null)
                        _timerAccelation.Interval = _accelationInterval;
                }
            }
        }
        [Category("Action")]
        [DefaultValue(400)]
        public int AccelationIntervalHighSpeed
        {
            get { return _accelationIntervalHighSpeed; }
            set
            {
                if (_accelationIntervalHighSpeed != value)
                {
                    _accelationIntervalHighSpeed = value;
                }
            }
        }
        #endregion
        #region Hidden Properties
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Image BackgroundImage { get; set; }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ImageLayout BackgroundImageLayout { get; set; }
        #endregion

        #region Private Method
        private void ProcessRadioButton()
        {
            if (Parent == null)
                return;
            foreach (Control ct in this.Parent.Controls)
            {
                if (ct is WButton)
                {
                    WButton bt = (WButton)ct;
                    if (bt.ButtonType == EnButtonType.RadioButton)
                    {
                        if (bt == this)
                            continue;
                        else
                        {
                            bt._pushed = false;
                            bt.Invalidate();
                        }
                    }
                }
            }
        }
        private void _scrollTimer_Tick(object sender, EventArgs e)
        {
            _scrollingValue += _scrollingOffset;
            this.Invalidate();
        }
        private void PaintBackGround(PaintEventArgs e)
        {
            if (e == null)
                return;
            if (e.Graphics == null)
                return;
            if (ButtonColor == EnButtonColor.Solid && ButtonSolidColor == Color.Transparent && !_pushed)
                return;
            if (PushedColor == EnButtonColor.Solid && PushedSolidColor == Color.Transparent && _pushed)
                return;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            RectangleF rect = new RectangleF(0, 0, Size.Width, Size.Height);
            ColorBlend blend = new ColorBlend();
            if (!this.Enabled)
            {
                blend.Colors = SolidColor(_disabledSolidColor);
                blend.Positions = GetPosition(EnButtonColor.Solid);
            }
            else if (_mouseDown)
            {
                if (_mouseDownColor == EnButtonColor.Solid && _mouseDownSolidColor != Color.Empty)
                {
                    blend.Colors = SolidColor(_mouseDownSolidColor);
                    blend.Positions = GetPosition(EnButtonColor.Solid);
                }
                else
                {
                    blend.Colors = GetColor(_mouseDownColor);
                    blend.Positions = GetPosition(_mouseDownColor);
                }
            }
            else if (_pushed && (_buttonType != EnButtonType.CheckBox && _buttonType != EnButtonType.RightCheckBox))
            {
                if (_pushedColor == EnButtonColor.Solid && _mouseDownSolidColor != Color.Empty)
                {
                    blend.Colors = SolidColor(_pushedSolidColor);
                    blend.Positions = GetPosition(EnButtonColor.Solid);
                }
                else
                {
                    blend.Colors = GetColor(_pushedColor);
                    blend.Positions = GetPosition(_pushedColor);
                }
            }
            else
            {
                if (_buttonColor == EnButtonColor.Solid && _mouseDownSolidColor != Color.Empty)
                {
                    blend.Colors = SolidColor(_buttonSolidColor);
                    blend.Positions = GetPosition(EnButtonColor.Solid);
                }
                else
                {
                    blend.Colors = GetColor(_buttonColor);
                    blend.Positions = GetPosition(_buttonColor);
                }
            }
            LinearGradientBrush brush = new LinearGradientBrush(rect, Color.Black, Color.Black, LinearGradientMode.Vertical);
            brush.InterpolationColors = blend;
            GraphicsPath path = GetPath(rect, _radius);
            e.Graphics.FillPath(brush, path);
            brush.Dispose();
            brush = null;
            path.Dispose();
            path = null;
        }
        private GraphicsPath GetPath(Rectangle rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }
        private GraphicsPath GetPath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            return path;
        }
        private void PaintBorder(PaintEventArgs e)
        {
            if (e == null)
                return;
            if (e.Graphics == null)
                return;
            Rectangle rect = new Rectangle(0, 0, Size.Width, Size.Height);
            GraphicsPath path = GetPath(rect, _radius);
            e.Graphics.DrawPath(new Pen(_borderColor, _borderThick), path);
            path.Dispose();
            path = null;
        }
        private void PaintChecker(PaintEventArgs e)
        {
            if (_buttonType != EnButtonType.CheckBox && _buttonType != EnButtonType.RightCheckBox)
                return;
            if (e == null)
                return;
            if (e.Graphics == null)
                return;
            if (_arrowColor == EnButtonColor.Solid && _arrowSolidColor == Color.Transparent && !_pushed)
                return;
            if (_pushedArrowColor == EnButtonColor.Solid && _pushedArrowSolidColor == Color.Transparent && _pushed)
                return;

            RectangleF rect = GetCheckBox();
            ColorBlend blend = new ColorBlend();
            blend.Colors = _checkerBoxColor == EnButtonColor.Solid ? SolidColor(_checkerBoxSolidColor) : GetColor(EnButtonColor.WhiteGradient);
            blend.Positions = GetPosition(EnButtonColor.WhiteGradient);
            LinearGradientBrush brush = new LinearGradientBrush(rect, Color.Black, Color.Black, LinearGradientMode.Vertical);
            brush.InterpolationColors = blend;
            GraphicsPath path = GetPath(rect, _radius);
            e.Graphics.FillPath(brush, path);
            e.Graphics.DrawPath(new Pen(_borderColor, _checkBoxThick), path);
            brush.Dispose();
            brush = null;
            if (_pushed)
            {
                ColorBlend tickBlend = new ColorBlend();
                if (Enabled)
                {
                    tickBlend.Colors = _arrowColor == EnButtonColor.Solid ? SolidColor(_arrowSolidColor) : GetColor(_arrowColor);
                    tickBlend.Positions = GetPosition(_arrowColor);
                }
                else
                {
                    tickBlend.Colors = SolidColor(_disabledArrowColor);
                    tickBlend.Positions = GetPosition(EnButtonColor.Solid);
                }

                LinearGradientBrush tickBrush = new LinearGradientBrush(rect, Color.Black, Color.Black, LinearGradientMode.Vertical);
                tickBrush.InterpolationColors = tickBlend;
                GraphicsPath tickPath = new GraphicsPath();
                PointF[] pt = new PointF[]
                {
                    new PointF(rect.Left, rect.Height * 0.5f),
                    new PointF(rect.Left + rect.Width * 0.25f, rect.Height * 0.5f),
                    new PointF(rect.Left + rect.Width * 0.45f, rect.Height * 0.65f),
                    new PointF(rect.Left + rect.Width * 0.75f, rect.Height * 0.1f),
                    new PointF(rect.Left + rect.Width * 0.95f, rect.Height * 0.1f),
                    new PointF(rect.Left + rect.Width * 0.48f, rect.Height * 0.9f),
                    new PointF(rect.Left, rect.Height * 0.5f),
                };
                tickPath.AddLines(pt);//, 0.2f);
                e.Graphics.FillPath(tickBrush, tickPath);
            }
        }
        private void PaintImage(PaintEventArgs e)
        {
            if (_image == null)
                return;
            if (e == null)
                return;
            if (e.Graphics == null)
                return;
            RectangleF srcRectangle = new RectangleF(0, 0, _image.Width, _image.Height);
            RectangleF dstRectanlge = GetDestRectangle();
            if (!this.Enabled && DisabledImage != null)
                e.Graphics.DrawImage(this.DisabledImage, dstRectanlge, srcRectangle, GraphicsUnit.Pixel);
            else if ((_pushed || _mouseDown) && _pushedImage != null)
                e.Graphics.DrawImage(this.PushedImage, dstRectanlge, srcRectangle, GraphicsUnit.Pixel);
            else
            {
                if (this._isAnimating)
                    ImageAnimator.UpdateFrames(this.Image);
                e.Graphics.DrawImage(this.Image, dstRectanlge, srcRectangle, GraphicsUnit.Pixel);
            }
        }
        private void PaintText(PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
                return;
            if (e == null)
                return;
            if (e.Graphics == null)
                return;
            Font font = _shiftText ? this.SubTextFont : this.Font;
            Color innerforecolor;
            Color outerforecolor;
            if (Enabled)
            {
                innerforecolor = _mouseDown || _pushed ? PushedInnerForeColor : InnerForeColor;
                outerforecolor = _mouseDown || _pushed ? PushedOuterForeColor : OuterForeColor;
            }
            else
            {
                innerforecolor = DisabledInnerForeColor;
                outerforecolor = DisabledOuterForeColor;
            }
            string text = this.Text;
            float deadLine = 0;
            if (this.ButtonType == EnButtonType.NumericUpDown)
                text = _value.ToString(string.Format("F{0}", _decimalPlaces.ToString()));
            text = !string.IsNullOrEmpty(SubText) && _shiftText ? this.SubText : text;
            StringFormat format = new StringFormat();
            if (ScrollText)
                format.FormatFlags = format.FormatFlags | StringFormatFlags.NoWrap;
            else
                format.FormatFlags = format.FormatFlags | StringFormatFlags.NoClip;
            switch (_textAlignment)
            {
                case ContentAlignment.TopLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Far;
                    break;
            }

            GraphicsPath path = new GraphicsPath();
            RectangleF rect = new RectangleF(this.Padding.Left + this.TextPadding.Left, this.Padding.Top + this.TextPadding.Top, this.Width - this.Padding.Horizontal - this.TextPadding.Horizontal, this.Height - this.Padding.Vertical - this.TextPadding.Vertical);
            RectangleF box = new RectangleF();
            if (_buttonType == EnButtonType.NumericUpDown)
            {
                box = GetNumericUpdown();
                rect.Width = rect.Width - box.Width;
            }
            else if (_buttonType == EnButtonType.CheckBox)
            {
                box = GetCheckBox();
                rect.X = rect.X + box.Width;
                rect.Width = rect.Width - box.Width;
            }
            else if (_buttonType == EnButtonType.RightCheckBox)
            {
                box = GetCheckBox();
                rect.Width = rect.Width - box.Width;
            }
            else if (!_textOnImage && _image != null)
            {
                box = GetDestRectangle();
                if (_centerImage)
                {
                    switch (_textAlignment)
                    {
                        case ContentAlignment.TopLeft:
                            rect.Height = box.Top - Padding.Top - TextPadding.Vertical;
                            rect.Width = box.Left - Padding.Left - TextPadding.Horizontal;
                            break;
                        case ContentAlignment.TopCenter:
                            rect.Height = box.Top - Padding.Top - TextPadding.Vertical;
                            rect.X = box.X + TextPadding.Left;
                            rect.Width = box.Width - TextPadding.Horizontal;
                            break;
                        case ContentAlignment.TopRight:
                            rect.Height = box.Top - Padding.Top - TextPadding.Vertical;
                            rect.X = box.Right + TextPadding.Left;
                            rect.Width = this.Width - box.Right - Padding.Right - TextPadding.Horizontal;
                            break;
                        case ContentAlignment.MiddleLeft:
                            rect.Y = box.Top + TextPadding.Top;
                            rect.Width = box.Left - Padding.Left - TextPadding.Horizontal;
                            rect.Height = box.Height - TextPadding.Vertical;
                            break;
                        case ContentAlignment.MiddleCenter:
                            format.Alignment = StringAlignment.Center;
                            format.LineAlignment = StringAlignment.Near;
                            rect.Y = box.Bottom + TextPadding.Top;
                            rect.X = box.Left + TextPadding.Left;
                            rect.Width = box.Width - TextPadding.Horizontal;
                            rect.Height = this.Height - box.Bottom - Padding.Vertical - TextPadding.Vertical;
                            break;
                        case ContentAlignment.MiddleRight:
                            rect.Y = box.Top + TextPadding.Top;
                            rect.X = box.Right + TextPadding.Left;
                            rect.Width = this.Width - box.Right - Padding.Right - TextPadding.Horizontal;
                            rect.Height = box.Height - TextPadding.Vertical;
                            break;
                        case ContentAlignment.BottomLeft:
                            rect.Y = box.Bottom + TextPadding.Top;
                            rect.Width = box.Left - Padding.Left - TextPadding.Horizontal;
                            rect.Height = this.Height - box.Bottom - Padding.Bottom - TextPadding.Vertical;
                            break;
                        case ContentAlignment.BottomCenter:
                            rect.Y = box.Bottom + TextPadding.Top;
                            rect.X = box.Left + TextPadding.Left;
                            rect.Width = box.Width - TextPadding.Horizontal;
                            rect.Height = this.Height - box.Bottom - Padding.Bottom - TextPadding.Vertical;
                            break;
                        case ContentAlignment.BottomRight:
                            rect.Y = box.Bottom + TextPadding.Top;
                            rect.Width = this.Width - box.Right - Padding.Right - TextPadding.Horizontal;
                            rect.Height = this.Height - box.Bottom - Padding.Bottom - TextPadding.Vertical;
                            rect.X = box.Right + TextPadding.Left;
                            break;
                    }
                }
                else
                {
                    rect.X = box.Right + TextPadding.Left;
                    rect.Width = this.Width - box.Right - Padding.Right - TextPadding.Horizontal;
                }
            }
            if (AutoSizeFont)
                font = AdjustFont(e.Graphics, text, font, rect.Size);
            else
            {
                RectangleF size = GetStringBound(text, font, this.Bounds, format);
                if (ScrollText && (size.Width * _flowTextLengthOffset > rect.Width))
                {
                    format.Alignment = StringAlignment.Near;
                    if (_scrollingValue > size.Width * _flowTextLengthOffset - rect.Width)
                        _scrollingValue = 0;
                    e.Graphics.Clip = new Region(rect);
                    rect.X -= _scrollingValue;
                    rect.Width += _scrollingValue;
                }
            }
            path.AddString(text, font.FontFamily, (int)font.Style, font.Size, rect, format);
            e.Graphics.FillPath(new SolidBrush(innerforecolor), path);
            e.Graphics.DrawPath(new Pen(outerforecolor, _outerTextThick), path);
            e.Graphics.Clip.Dispose();
            path = null;
            font = null;
            format = null;
        }
        private RectangleF GetStringBound(string text, Font font, RectangleF bound, StringFormat format)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddString(text, font.FontFamily, (int)font.Style, font.Size, bound, format);
                return path.GetBounds();
            }
        }

        private void PaintSubText(PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(this.SubText))
                return;
            if (e == null)
                return;
            if (e.Graphics == null)
                return;
            if (!SubTextVisible)
                return;
            RectangleF rect = new RectangleF(this.Padding.Left + this.SubTextPadding.Left, this.Padding.Top + this.SubTextPadding.Top, this.Width - this.Padding.Horizontal - this.SubTextPadding.Horizontal, this.Height - this.Padding.Vertical - this.SubTextPadding.Vertical);
            StringFormat format = new StringFormat();
            Color innerforecolor;
            Color outerforecolor;
            if (Enabled)
            {
                innerforecolor = _mouseDown || _pushed ? PushedInnerSubTextColor : InnerSubTextColor;
                outerforecolor = _mouseDown || _pushed ? PushedOuterSubTextColor : OuterSubTextColor;
            }
            else
            {
                innerforecolor = DisabledInnerSubTextColor;
                outerforecolor = DisabledOuterSubTextColor;
            }
            switch (_subTextAlignment)
            {
                case ContentAlignment.TopLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Far;
                    break;
            }
            using (GraphicsPath path = new GraphicsPath())
            {
                string tmpText = _shiftText ? this.Text : this.SubText;
                path.AddString(tmpText, _subTextFont.FontFamily, (int)_subTextFont.Style, _subTextFont.Size, rect, format);
                e.Graphics.FillPath(new SolidBrush(innerforecolor), path);
                e.Graphics.DrawPath(new Pen(outerforecolor, _outerSubTextThick), path);
            }
            format = null;
        }
        private void PaintUnderLine(PaintEventArgs e)
        {
            if (e == null)
                return;
            if (e.Graphics == null)
                return;
            if (_underLine == EnUnderLine.None)
                return;
            Rectangle rect = new Rectangle(0, this.Height - _underLineThick, this.Width, this.Height);
            if (_shadow.HasFlag(EnShadow.Side))
                rect.Height = -7;
            if (_shadow.HasFlag(EnShadow.Bottom))
                rect.Width = -7;
            if (_underLineMouseOver != Color.Empty && ClientRectangle.Contains(PointToClient(Control.MousePosition)))
            {
                e.Graphics.FillRectangle(new SolidBrush(_underLineMouseOver), rect);
                return;
            }
            else if (_underLine.HasFlag(EnUnderLine.Normal) && !this.Pushed && !_mouseDown)
            {
                e.Graphics.FillRectangle(new SolidBrush(_underLineColor), rect);
                return;
            }
            else if (_underLine.HasFlag(EnUnderLine.Pushed) && (this.Pushed || _mouseDown))
            {
                rect = new Rectangle(0, this.Height - _pushedUnderLineThick, this.Width, this.Height);
                e.Graphics.FillRectangle(new SolidBrush(_pushedUnderLineColor), rect);
                return;
            }
        }
        private void PaintShadow(PaintEventArgs e)
        {
            if (e == null)
                return;
            if (e.Graphics == null)
                return;
            if (_shadow == EnShadow.None)
                return;
            Color[] shadow = GetShadow();
            using (Pen pen = new Pen(shadow[0], 1))
            {
                for (int i = 0; i < shadow.Length; i++)
                {
                    pen.Color = shadow[i];
                    if (_shadow.HasFlag(EnShadow.Both))
                    {
                        e.Graphics.DrawLine(pen, 0, this.Height - i, this.Width - i, this.Height - i);
                        e.Graphics.DrawLine(pen, this.Width - i, 0, this.Width - i, this.Height - i);
                    }
                    else if (_shadow.HasFlag(EnShadow.Bottom))
                        e.Graphics.DrawLine(pen, 0, this.Height - i, this.Width, this.Height - i);
                    else if (_shadow.HasFlag(EnShadow.Side))
                        e.Graphics.DrawLine(pen, this.Width - i, 0, this.Width - i, this.Height);
                }
            }
        }
        public Font AdjustFont(Graphics g, string str, Font origin, SizeF containerSize)
        {
            for (int adjustedSize = (int)origin.Size; adjustedSize >= 1; adjustedSize--)
            {
                Font retFont = new Font(origin.Name, adjustedSize, origin.Style, GraphicsUnit.Pixel);
                SizeF newSize = g.MeasureString(str, retFont, (int)containerSize.Width);
                if (containerSize.Height > Convert.ToInt32(newSize.Height))
                    return retFont;
            }
            return new Font(origin.Name, 1, origin.Style, GraphicsUnit.Pixel);
        }
        public void PaintNumeric(PaintEventArgs e)
        {
            if (e == null)
                return;
            if (e.Graphics == null)
                return;
            if (_buttonType != EnButtonType.NumericUpDown)
                return;
            RectangleF rect = GetNumericUpdown();
            PointF[] right;
            PointF[] left;

            left = new PointF[] { new PointF(rect.Left + rect.Width * 0.4f, rect.Height * 0.15f), new PointF(rect.Left + rect.Width * 0.1f, rect.Height / 2), new PointF(rect.Left + rect.Width * 0.4f, rect.Height * 0.85f) };
            right = new PointF[] { new PointF(rect.Left + rect.Width * 0.6f, rect.Height * 0.15f), new PointF(rect.Left + rect.Width * 0.9f, rect.Height / 2), new PointF(rect.Left + rect.Width * 0.6f, rect.Height * 0.85f) };

            ColorBlend leftBlend = new ColorBlend();
            ColorBlend rightBlend = new ColorBlend();
            if (!Enabled)
            {
                leftBlend.Colors = SolidColor(_disabledArrowColor);
                leftBlend.Positions = GetPosition(EnButtonColor.Solid);
                rightBlend.Colors = SolidColor(_disabledArrowColor);
                rightBlend.Positions = GetPosition(EnButtonColor.Solid);
            }
            else if (_leftPushed)
            {
                leftBlend.Colors = _pushedArrowColor == EnButtonColor.Solid ? SolidColor(_pushedArrowSolidColor) : GetColor(_pushedArrowColor);
                leftBlend.Positions = GetPosition(_pushedArrowColor);
                rightBlend.Colors = _arrowColor == EnButtonColor.Solid ? SolidColor(_arrowSolidColor) : GetColor(_arrowColor);
                rightBlend.Positions = GetPosition(_arrowColor);
            }
            else if (_rightPushed)
            {
                leftBlend.Colors = _arrowColor == EnButtonColor.Solid ? SolidColor(_arrowSolidColor) : GetColor(_arrowColor);
                leftBlend.Positions = GetPosition(_arrowColor);
                rightBlend.Colors = _pushedArrowColor == EnButtonColor.Solid ? SolidColor(_pushedArrowSolidColor) : GetColor(_pushedArrowColor);
                rightBlend.Positions = GetPosition(_pushedArrowColor);
            }
            else
            {
                leftBlend.Colors = _arrowColor == EnButtonColor.Solid ? SolidColor(_arrowSolidColor) : GetColor(_arrowColor);
                leftBlend.Positions = GetPosition(_arrowColor);
                rightBlend.Colors = _arrowColor == EnButtonColor.Solid ? SolidColor(_arrowSolidColor) : GetColor(_arrowColor);
                rightBlend.Positions = GetPosition(_arrowColor);
            }
            System.Drawing.Drawing2D.LinearGradientBrush leftBrush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.Black, Color.Black, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            System.Drawing.Drawing2D.LinearGradientBrush rightBrush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.Black, Color.Black, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            leftBrush.InterpolationColors = leftBlend;
            rightBrush.InterpolationColors = rightBlend;
            e.Graphics.FillRectangle(new SolidBrush(Color.White), rect);
            e.Graphics.FillPolygon(leftBrush, left);
            e.Graphics.FillPolygon(rightBrush, right);
            leftBrush.Dispose();
            leftBrush = null;
            rightBrush.Dispose();
            rightBrush = null;
        }

        private RectangleF GetDestRectangle()
        {
            if (_image == null)
                return RectangleF.Empty;
            float ratio = 1;
            RectangleF bound = new RectangleF(Padding.Left + ImagePadding.Left, Padding.Top + ImagePadding.Top, (this.Width * _imagePercentage) - Padding.Right - ImagePadding.Right, (this.Height * _imagePercentage) - Padding.Bottom - ImagePadding.Bottom);
            if (!_fitImage || _fixedRatio)
            {
                bound.Width = Image.Width * _imagePercentage;
                bound.Height = Image.Height * _imagePercentage;
            }
            if (_fixedRatio)
            {
                ratio = Math.Min((float)this.Width / (float)_image.Width, (float)this.Height / (float)_image.Height);
                if (!_fitImage)
                    ratio = Math.Min(1f, ratio);
                bound.Width = bound.Width * ratio;
                bound.Height = bound.Height * ratio;
            }
            if (_centerImage)
            {
                bound.X += (this.Width - bound.Width) / 2;
                bound.Y += (this.Height - bound.Height) / 2;
            }
            return bound;
        }
        private RectangleF GetNumericUpdown()
        {
            int width = Math.Min(this.Height, this.Width / 2);
            return new RectangleF(this.Width - width, 0, width, this.Height);
        }
        private RectangleF GetCheckBox()
        {
            int width = Math.Min(this.Height, this.Width / 2);
            if (ButtonType == EnButtonType.CheckBox)
                return new RectangleF(0, 0, width, this.Height);
            else
                return new RectangleF(this.Width - width, 0, width, this.Height);
        }


        private Color[] GetColor(EnButtonColor _color)
        {
            switch (_color)
            {
                case EnButtonColor.GrayGradient1:
                    return new Color[] { Color.FromArgb(_alpha, 122, 122, 122), Color.FromArgb(_alpha, 103, 103, 103), Color.FromArgb(_alpha, 85, 85, 85), Color.FromArgb(_alpha, 42, 42, 42), Color.FromArgb(_alpha, 32, 32, 32) };
                case EnButtonColor.GrayGradient2:
                    return new Color[] { Color.FromArgb(_alpha, 156, 156, 156), Color.FromArgb(_alpha, 140, 140, 140), Color.FromArgb(_alpha, 127, 127, 127), Color.FromArgb(_alpha, 66, 66, 66), Color.FromArgb(_alpha, 52, 52, 52) };
                case EnButtonColor.GrayGradient3:
                    return new Color[] { Color.FromArgb(_alpha, 170, 170, 170), Color.FromArgb(_alpha, 157, 157, 157), Color.FromArgb(_alpha, 146, 146, 146), Color.FromArgb(_alpha, 96, 96, 96), Color.FromArgb(_alpha, 77, 77, 77) };
                case EnButtonColor.GrayGradient4:
                    return new Color[] { Color.FromArgb(_alpha, 197, 197, 197), Color.FromArgb(_alpha, 180, 180, 180), Color.FromArgb(_alpha, 126, 126, 126), Color.FromArgb(_alpha, 115, 115, 115), Color.FromArgb(_alpha, 102, 102, 102) };
                case EnButtonColor.GreenGradient:
                    return new Color[] { Color.FromArgb(_alpha, 54, 220, 148), Color.FromArgb(_alpha, 0, 209, 118), Color.FromArgb(_alpha, 16, 139, 59), Color.FromArgb(_alpha, 16, 126, 53), Color.FromArgb(_alpha, 15, 111, 47) };
                case EnButtonColor.AmberGradient:
                    return new Color[] { Color.FromArgb(_alpha, 255, 195, 54), Color.FromArgb(_alpha, 255, 179, 0), Color.FromArgb(_alpha, 255, 123, 0), Color.FromArgb(_alpha, 235, 110, 0), Color.FromArgb(_alpha, 209, 98, 0) };
                case EnButtonColor.RedGradient:
                    return new Color[] { Color.FromArgb(_alpha, 236, 99, 117), Color.FromArgb(_alpha, 227, 55, 81), Color.FromArgb(_alpha, 127, 10, 20), Color.FromArgb(_alpha, 115, 8, 17), Color.FromArgb(_alpha, 102, 8, 14) };
                case EnButtonColor.BlueGradient:
                    return new Color[] { Color.FromArgb(_alpha, 0, 105, 158), Color.FromArgb(_alpha, 0, 131, 196), Color.FromArgb(_alpha, 0, 154, 237), Color.FromArgb(_alpha, 0, 92, 162), Color.FromArgb(_alpha, 15, 101, 167) };
                case EnButtonColor.WhiteGradient:
                    return new Color[] { Color.FromArgb(_alpha, 119, 119, 119), Color.FromArgb(_alpha, 172, 172, 172), Color.FromArgb(_alpha, 225, 225, 225), Color.FromArgb(_alpha, 244, 244, 244), Color.FromArgb(_alpha, 248, 248, 248) };
                default:
                    return null;
            }
        }
        private Color[] SolidColor(Color color)
        {
            return new Color[] { Color.FromArgb(_alpha, color.R, color.G, color.B), Color.FromArgb(_alpha, color.R, color.G, color.B) };
        }
        private float[] GetPosition(EnButtonColor _color)
        {
            switch (_color)
            {
                case EnButtonColor.GrayGradient1:
                    return new float[] { 0.0f, 0.02f, 0.04f, 0.96f, 1.0f };
                case EnButtonColor.GrayGradient2:
                    return new float[] { 0.0f, 0.02f, 0.04f, 0.96f, 1.0f };
                case EnButtonColor.GrayGradient3:
                    return new float[] { 0.0f, 0.02f, 0.04f, 0.94f, 1.0f };
                case EnButtonColor.GrayGradient4:
                    return new float[] { 0.0f, 0.05f, 0.96f, 0.98f, 1.0f };
                case EnButtonColor.GreenGradient:
                    return new float[] { 0.0f, 0.07f, 0.96f, 0.98f, 1.0f };
                case EnButtonColor.AmberGradient:
                    return new float[] { 0.0f, 0.07f, 0.96f, 0.98f, 1.0f };
                case EnButtonColor.RedGradient:
                    return new float[] { 0.0f, 0.07f, 0.96f, 0.98f, 1.0f };
                case EnButtonColor.BlueGradient:
                    return new float[] { 0.0f, 0.04f, 0.09f, 0.96f, 1.0f };
                case EnButtonColor.WhiteGradient:
                    return new float[] { 0.0f, 0.05f, 0.13f, 0.21f, 1.0f };
                default:
                    return new float[] { 0.0f, 1.0f };
            }
        }
        private Color[] GetShadow()
        {
            return new Color[]
            {
                Color.FromArgb(250, 250, 250),
                Color.FromArgb(240, 240, 240),
                Color.FromArgb(225, 225, 225),
                Color.FromArgb(210, 210, 210),
                Color.FromArgb(195, 195, 195),
                Color.FromArgb(195, 195, 195),
                Color.FromArgb(180, 180, 180),
                Color.FromArgb(180, 180, 180)
            };
        }
        private float[] GetShadowPosition()
        {
            return new float[] { 0.125f, 0.25f, 0.375f, 0.50f, 0.625f, 0.75f, 0.875f, 1.0f };
        }

        private void OnFrameChangedHandler(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        #endregion
        #region Public Method
        public void SetValue(double value)
        {
            if (_value != value)
            {
                if (value >= _maxValue)
                    _value = _maxValue;
                else if (value <= _minValue)
                    _value = _minValue;
                else
                    _value = value;
                this.Text = _value.ToString(string.Format("F{0}", _decimalPlaces.ToString()));
            }
        }

        #endregion
        #region Override Method
        protected override void OnPaint(PaintEventArgs e)
        {
            PaintBackGround(e);
            PaintChecker(e);
            PaintImage(e);
            PaintText(e);
            PaintSubText(e);
            PaintNumeric(e);
            PaintBorder(e);
            PaintUnderLine(e);
            PaintShadow(e);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._isAnimating)
                    ImageAnimator.StopAnimate(this._image, this.OnFrameChangedHandler);
                if (_timerAccelation != null)
                {
                    _timerAccelation.Stop();
                    _timerAccelation.Dispose();
                    _timerAccelation = null;
                }
                if (_scrollTimer != null)
                {
                    _scrollTimer.Stop();
                    _scrollTimer.Dispose();
                    _scrollTimer = null;
                }
                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }
                if (_pushedImage != null)
                {
                    _pushedImage.Dispose();
                    _pushedImage = null;
                }
                if (_disabledImage != null)
                {
                    _disabledImage.Dispose();
                    _disabledImage = null;
                }
            }
            base.Dispose(disposing);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.Enabled)
                return;
            if (this.ButtonType == EnButtonType.Label)
            {
                base.OnMouseDown(e);
                return;
            }
            if (this.ButtonType == EnButtonType.RadioButton && Pushed)
                return;
            if (this.ButtonType == EnButtonType.NumericUpDown)
            {
                RectangleF rect = GetNumericUpdown();
                _previousValue = _value;
                if (rect.Contains(e.Location))
                {
                    if (e.X < rect.Left + (rect.Width / 2))
                    {
                        _leftPushed = true;
                        if (e.Button == MouseButtons.Left)
                        {
                            _leftDown = true;
                            _value -= _increasment;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            _rightDown = true;
                            _value -= _increasmentOnRightClick;
                        }
                        if (_value < _minValue)
                            _value = _minValue;
                        else
                        {
                            _timerAccelation.Start();
                            _numericCount = 0;
                        }
                    }
                    else
                    {
                        _rightPushed = true;
                        if (e.Button == MouseButtons.Left)
                        {
                            _leftDown = true;
                            _value += _increasment;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            _rightDown = true;
                            _value += _increasmentOnRightClick;
                        }
                        if (_value > _maxValue)
                            _value = _maxValue;
                        else
                        {
                            _timerAccelation.Start();
                            _numericCount = 0;
                        }
                    }
                }
                else
                    _mouseDown = true;
            }
            else
                _mouseDown = true;
            this.Invalidate();
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!this.Enabled)
                return;
            if (this.ButtonType == EnButtonType.Label)
                return;
            if (ClientRectangle.Contains(PointToClient(Control.MousePosition)))
            {
                switch (ButtonType)
                {
                    case EnButtonType.NormalButton:
                        Pushed = true;
                        break;
                    case EnButtonType.CheckBox:
                    case EnButtonType.RightCheckBox:
                    case EnButtonType.PushButton:
                        Pushed = !_pushed;
                        break;
                    case EnButtonType.RadioButton:
                        if (!_pushed)
                            Pushed = true;
                        break;
                    case EnButtonType.NumericUpDown:
                        if (_previousValue != _value)
                        {
                            _timerAccelation.Stop();
                            OnValueChanged(EventArgs.Empty);
                        }
                        if (!_leftPushed && !_rightPushed)
                            Pushed = true;
                        break;
                }
            }
            if (_mouseDown)
                _mouseDown = false;
            _leftPushed = false;
            _rightPushed = false;
            base.OnMouseUp(e);
            Invalidate();
        }
        #endregion
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            this.Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Invalidate();
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.Invalidate();
        }
    }
    #region Enum
    public enum EnButtonColor
    {
        GreenGradient,
        AmberGradient,
        RedGradient,
        BlueGradient,
        GrayGradient1,
        GrayGradient2,
        GrayGradient3,
        GrayGradient4,
        WhiteGradient,
        Solid,
    }
    public enum EnButtonType
    {
        NormalButton,
        PushButton,
        CheckBox,
        RightCheckBox,
        Label,
        NumericUpDown,
        RadioButton,
    }
    public enum EnUnderLine
    {
        None = 0,
        Normal = 1 << 0,
        Pushed = 1 << 1,
        Both = Normal | Pushed,

    }
    public enum EnButtonPressed
    {
        Whole,
        Left,
        Right,
    }
    public enum EnArrowDirection
    {
        TopBottom,
        LeftRight,
    }
    public enum EnShadow
    {
        None = 0,
        Side = 1 << 0,
        Bottom = 1 << 1,
        Both = Side | Bottom,
    }
    #endregion

}
