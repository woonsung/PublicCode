
namespace PublicCode
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.wButton1 = new PublicCommonControls.WButton.WButton();
            this.SuspendLayout();
            // 
            // wButton1
            // 
            this.wButton1.AbsoluteSubTextOffset = true;
            this.wButton1.AccelationIntervalHighSpeed = 50;
            this.wButton1.Alpha = 255;
            this.wButton1.ArrowColor = PublicCommonControls.WButton.EnButtonColor.GrayGradient2;
            this.wButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.wButton1.ButtonColor = PublicCommonControls.WButton.EnButtonColor.GrayGradient1;
            this.wButton1.ButtonSolidColor = System.Drawing.Color.Empty;
            this.wButton1.DrawBorder = true;
            this.wButton1.DrawBorderOnDisabled = true;
            this.wButton1.FitImage = true;
            this.wButton1.Image = ((System.Drawing.Image)(resources.GetObject("wButton1.Image")));
            this.wButton1.Increasement = 1D;
            this.wButton1.IncreasementOnRightClick = 10D;
            this.wButton1.InnerSubTextColor = System.Drawing.Color.Black;
            this.wButton1.Location = new System.Drawing.Point(41, 42);
            this.wButton1.MaxValue = 100D;
            this.wButton1.MinValue = 0D;
            this.wButton1.MouseDownColor = PublicCommonControls.WButton.EnButtonColor.BlueGradient;
            this.wButton1.Name = "wButton1";
            this.wButton1.OuterSubTextColor = System.Drawing.Color.Black;
            this.wButton1.Pushed = false;
            this.wButton1.PushedColor = PublicCommonControls.WButton.EnButtonColor.Solid;
            this.wButton1.PushedInnerForeColor = System.Drawing.Color.Black;
            this.wButton1.PushedOuterForeColor = System.Drawing.Color.Black;
            this.wButton1.PushedSolidColor = System.Drawing.Color.White;
            this.wButton1.PushedUnderLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.wButton1.Radius = 10;
            this.wButton1.Size = new System.Drawing.Size(1033, 642);
            this.wButton1.SubTextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.wButton1.TabIndex = 0;
            this.wButton1.Text = "wButton1";
            this.wButton1.UnderLineMouseOverColor = System.Drawing.Color.Empty;
            this.wButton1.Value = 0D;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1531, 942);
            this.Controls.Add(this.wButton1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private PublicCommonControls.WButton.WButton wButton1;
    }
}

