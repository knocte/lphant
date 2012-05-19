#region Copyright (c)2003 Juanjo < http://lphant.sourceforge.net >
/*
* This file is part of eLePhant
* Copyright (C)2003 Juanjo < j_u_a_n_j_o@users.sourceforge.net / http://lphant.sourceforge.net >
* 
* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either
* version 2 of the License, or (at your option) any later version.
* 
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
* 
* You should have received a copy of the GNU General Public License
* along with this program; if not, write to the Free Software
* Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using eLePhant.eDonkey;
using eLePhant.Types;
using Crownwood.Magic.Controls;

namespace eLePhant.Client
{
	/// <summary>
	/// Descripción breve de FormAviDetails.
	/// </summary>
	public class FormAviDetails : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox tx_VCodec;
		private System.Windows.Forms.TextBox tx_VFps;
		private System.Windows.Forms.TextBox tx_VBitrate;
		private System.Windows.Forms.TextBox tx_Length;
		private System.Windows.Forms.TextBox tx_Width;
		private System.Windows.Forms.TextBox tx_Height;
		private System.Windows.Forms.TextBox tx_ACodec;
		private System.Windows.Forms.TextBox tx_AChannels;
		private System.Windows.Forms.TextBox tx_ABitrate;
		private System.Windows.Forms.TextBox tx_ASampleRate;
		private System.Windows.Forms.Label labelVideoCodec;
		private System.Windows.Forms.Label labelVideoInfo;
		private System.Windows.Forms.Label labelVideoBitrate;
		private System.Windows.Forms.Label labelAudioInfo;
		private System.Windows.Forms.Label labelAudioCodec;
		private System.Windows.Forms.Label labelAudioBitrate;
		private System.Windows.Forms.Label labelVideoFPS;
		private System.Windows.Forms.Label labelVideoLength;
		private System.Windows.Forms.Label labelVideoWidth;
		private System.Windows.Forms.Label labelVideoHeight;
		private System.Windows.Forms.Label labelAudioChannels;
		private System.Windows.Forms.Label labelAudioSampleRate;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormAviDetails()
		{
			//
			// Necesario para admitir el Diseñador de Windows Forms
			//
			InitializeComponent();
			m_Globalize();

			//
			// TODO: agregar código de constructor después de llamar a InitializeComponent
			//
		}
		
		private void m_Globalize()
		{
			labelVideoInfo.Text = eLePhantForm.Globalization["LBL_VIDEOINFO"];
			labelVideoCodec.Text = eLePhantForm.Globalization["LBL_VIDEOCODEC"]+":";
			labelVideoBitrate.Text = eLePhantForm.Globalization["LBL_VIDEOBITRATE"]+":";
			labelVideoWidth.Text = eLePhantForm.Globalization["LBL_VIDEOWIDTH"]+":";
			labelVideoHeight.Text = eLePhantForm.Globalization["LBL_VIDEOHEIGHT"]+":";
			labelVideoLength.Text = eLePhantForm.Globalization["LBL_VIDEOLENGTH"]+":";
			labelVideoFPS.Text = eLePhantForm.Globalization["LBL_VIDEOFPS"]+":";
			labelAudioBitrate.Text = eLePhantForm.Globalization["LBL_AUDIOBITRATE"]+":";
			labelAudioChannels.Text = eLePhantForm.Globalization["LBL_AUDIOCHANNELS"]+":";
			labelAudioCodec.Text = eLePhantForm.Globalization["LBL_AUDIOCODEC"]+":";
			labelAudioInfo.Text = eLePhantForm.Globalization["LBL_AUDIOINFO"];
			labelAudioSampleRate.Text = eLePhantForm.Globalization["LBL_AUDIOSAMPLERATE"]+":";
		}

		public void Globalize()
		{
			m_Globalize();
		}

		public void ShowData(Hashtable ListDetails)
		{
			this.tx_VCodec.Text		= (string)ListDetails[Constants.Avi.VCodec]; //FileInfo.VideoInfo.m_VCodec ;
			this.tx_Width.Text		= (string)ListDetails[Constants.Avi.Width]; //FileInfo.VideoInfo.m_Width.ToString() ;
			this.tx_Height.Text		= (string)ListDetails[Constants.Avi.Height]; //FileInfo.VideoInfo.m_Height.ToString() ;
			this.tx_Length.Text		= (string)ListDetails[Constants.Avi.Length]; //FileInfo.VideoInfo.m_Length.ToString() ;
			this.tx_VFps.Text		= (string)ListDetails[Constants.Avi.VFps]; //FileInfo.VideoInfo.m_VFps.ToString();
			this.tx_VBitrate.Text	= (string)ListDetails[Constants.Avi.VBitrate]; //FileInfo.VideoInfo.m_VBitrate.ToString();
			this.tx_ACodec.Text		= (string)ListDetails[Constants.Avi.ACodec]; //FileInfo.VideoInfo.m_ACodec;
			this.tx_AChannels.Text	= (string)ListDetails[Constants.Avi.AChannel]; //FileInfo.VideoInfo.m_AChannel;
			this.tx_ABitrate.Text	= (string)ListDetails[Constants.Avi.ABitrate]; //FileInfo.VideoInfo.m_ABitrate.ToString();
			this.tx_ASampleRate.Text	= (string)ListDetails[Constants.Avi.ASampleRate]; //FileInfo.VideoInfo.m_ASampleRate.ToString();
		}

		/// <summary>
		/// Limpiar los recursos que se estén utilizando.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Código generado por el Diseñador de Windows Forms
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
			this.labelVideoCodec = new System.Windows.Forms.Label();
			this.labelVideoInfo = new System.Windows.Forms.Label();
			this.tx_VCodec = new System.Windows.Forms.TextBox();
			this.labelVideoFPS = new System.Windows.Forms.Label();
			this.tx_VFps = new System.Windows.Forms.TextBox();
			this.labelVideoBitrate = new System.Windows.Forms.Label();
			this.tx_VBitrate = new System.Windows.Forms.TextBox();
			this.labelVideoLength = new System.Windows.Forms.Label();
			this.tx_Length = new System.Windows.Forms.TextBox();
			this.labelVideoWidth = new System.Windows.Forms.Label();
			this.tx_Width = new System.Windows.Forms.TextBox();
			this.labelVideoHeight = new System.Windows.Forms.Label();
			this.tx_Height = new System.Windows.Forms.TextBox();
			this.labelAudioInfo = new System.Windows.Forms.Label();
			this.labelAudioCodec = new System.Windows.Forms.Label();
			this.tx_ACodec = new System.Windows.Forms.TextBox();
			this.labelAudioChannels = new System.Windows.Forms.Label();
			this.tx_AChannels = new System.Windows.Forms.TextBox();
			this.labelAudioBitrate = new System.Windows.Forms.Label();
			this.tx_ABitrate = new System.Windows.Forms.TextBox();
			this.labelAudioSampleRate = new System.Windows.Forms.Label();
			this.tx_ASampleRate = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// labelVideoCodec
			// 
			this.labelVideoCodec.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelVideoCodec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelVideoCodec.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelVideoCodec.ForeColor = System.Drawing.Color.Black;
			this.labelVideoCodec.Location = new System.Drawing.Point(0, 14);
			this.labelVideoCodec.Name = "labelVideoCodec";
			this.labelVideoCodec.Size = new System.Drawing.Size(59, 19);
			this.labelVideoCodec.TabIndex = 3;
			this.labelVideoCodec.Text = "Codec:";
			// 
			// labelVideoInfo
			// 
			this.labelVideoInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelVideoInfo.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelVideoInfo.ForeColor = System.Drawing.Color.Black;
			this.labelVideoInfo.Location = new System.Drawing.Point(0, 0);
			this.labelVideoInfo.Name = "labelVideoInfo";
			this.labelVideoInfo.Size = new System.Drawing.Size(379, 15);
			this.labelVideoInfo.TabIndex = 4;
			this.labelVideoInfo.Text = "VIDEO INFO";
			// 
			// tx_VCodec
			// 
			this.tx_VCodec.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_VCodec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_VCodec.Font = new System.Drawing.Font("Tahoma", 7F);
			this.tx_VCodec.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_VCodec.Location = new System.Drawing.Point(54, 14);
			this.tx_VCodec.Name = "tx_VCodec";
			this.tx_VCodec.ReadOnly = true;
			this.tx_VCodec.Size = new System.Drawing.Size(113, 19);
			this.tx_VCodec.TabIndex = 25;
			this.tx_VCodec.Text = "";
			// 
			// labelVideoFPS
			// 
			this.labelVideoFPS.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelVideoFPS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelVideoFPS.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelVideoFPS.ForeColor = System.Drawing.Color.Black;
			this.labelVideoFPS.Location = new System.Drawing.Point(166, 14);
			this.labelVideoFPS.Name = "labelVideoFPS";
			this.labelVideoFPS.Size = new System.Drawing.Size(51, 19);
			this.labelVideoFPS.TabIndex = 26;
			this.labelVideoFPS.Text = "Fps:";
			// 
			// tx_VFps
			// 
			this.tx_VFps.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_VFps.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_VFps.Font = new System.Drawing.Font("Tahoma", 7F);
			this.tx_VFps.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_VFps.Location = new System.Drawing.Point(215, 14);
			this.tx_VFps.Name = "tx_VFps";
			this.tx_VFps.ReadOnly = true;
			this.tx_VFps.Size = new System.Drawing.Size(58, 19);
			this.tx_VFps.TabIndex = 27;
			this.tx_VFps.Text = "";
			// 
			// labelVideoBitrate
			// 
			this.labelVideoBitrate.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelVideoBitrate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelVideoBitrate.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelVideoBitrate.ForeColor = System.Drawing.Color.Black;
			this.labelVideoBitrate.Location = new System.Drawing.Point(272, 14);
			this.labelVideoBitrate.Name = "labelVideoBitrate";
			this.labelVideoBitrate.Size = new System.Drawing.Size(59, 19);
			this.labelVideoBitrate.TabIndex = 28;
			this.labelVideoBitrate.Text = "Bitrate:";
			// 
			// tx_VBitrate
			// 
			this.tx_VBitrate.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_VBitrate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_VBitrate.Font = new System.Drawing.Font("Tahoma", 7F);
			this.tx_VBitrate.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_VBitrate.Location = new System.Drawing.Point(321, 14);
			this.tx_VBitrate.Name = "tx_VBitrate";
			this.tx_VBitrate.ReadOnly = true;
			this.tx_VBitrate.Size = new System.Drawing.Size(58, 19);
			this.tx_VBitrate.TabIndex = 29;
			this.tx_VBitrate.Text = "";
			// 
			// labelVideoLength
			// 
			this.labelVideoLength.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelVideoLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelVideoLength.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelVideoLength.ForeColor = System.Drawing.Color.Black;
			this.labelVideoLength.Location = new System.Drawing.Point(0, 32);
			this.labelVideoLength.Name = "labelVideoLength";
			this.labelVideoLength.Size = new System.Drawing.Size(59, 19);
			this.labelVideoLength.TabIndex = 30;
			this.labelVideoLength.Text = "Length:";
			// 
			// tx_Length
			// 
			this.tx_Length.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_Length.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_Length.Font = new System.Drawing.Font("Tahoma", 7F);
			this.tx_Length.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_Length.Location = new System.Drawing.Point(54, 32);
			this.tx_Length.Name = "tx_Length";
			this.tx_Length.ReadOnly = true;
			this.tx_Length.Size = new System.Drawing.Size(113, 19);
			this.tx_Length.TabIndex = 31;
			this.tx_Length.Text = "";
			// 
			// labelVideoWidth
			// 
			this.labelVideoWidth.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelVideoWidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelVideoWidth.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelVideoWidth.ForeColor = System.Drawing.Color.Black;
			this.labelVideoWidth.Location = new System.Drawing.Point(166, 32);
			this.labelVideoWidth.Name = "labelVideoWidth";
			this.labelVideoWidth.Size = new System.Drawing.Size(50, 19);
			this.labelVideoWidth.TabIndex = 32;
			this.labelVideoWidth.Text = "Width:";
			// 
			// tx_Width
			// 
			this.tx_Width.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_Width.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_Width.Font = new System.Drawing.Font("Tahoma", 7F);
			this.tx_Width.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_Width.Location = new System.Drawing.Point(215, 32);
			this.tx_Width.Name = "tx_Width";
			this.tx_Width.ReadOnly = true;
			this.tx_Width.Size = new System.Drawing.Size(58, 19);
			this.tx_Width.TabIndex = 33;
			this.tx_Width.Text = "";
			// 
			// labelVideoHeight
			// 
			this.labelVideoHeight.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelVideoHeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelVideoHeight.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelVideoHeight.ForeColor = System.Drawing.Color.Black;
			this.labelVideoHeight.Location = new System.Drawing.Point(272, 32);
			this.labelVideoHeight.Name = "labelVideoHeight";
			this.labelVideoHeight.Size = new System.Drawing.Size(50, 19);
			this.labelVideoHeight.TabIndex = 34;
			this.labelVideoHeight.Text = "Height:";
			// 
			// tx_Height
			// 
			this.tx_Height.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_Height.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_Height.Font = new System.Drawing.Font("Tahoma", 7F);
			this.tx_Height.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_Height.Location = new System.Drawing.Point(321, 32);
			this.tx_Height.Name = "tx_Height";
			this.tx_Height.ReadOnly = true;
			this.tx_Height.Size = new System.Drawing.Size(58, 19);
			this.tx_Height.TabIndex = 35;
			this.tx_Height.Text = "";
			// 
			// labelAudioInfo
			// 
			this.labelAudioInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelAudioInfo.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelAudioInfo.ForeColor = System.Drawing.Color.Black;
			this.labelAudioInfo.Location = new System.Drawing.Point(0, 50);
			this.labelAudioInfo.Name = "labelAudioInfo";
			this.labelAudioInfo.Size = new System.Drawing.Size(379, 15);
			this.labelAudioInfo.TabIndex = 36;
			this.labelAudioInfo.Text = "AUDIO INFO";
			// 
			// labelAudioCodec
			// 
			this.labelAudioCodec.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelAudioCodec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelAudioCodec.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelAudioCodec.ForeColor = System.Drawing.Color.Black;
			this.labelAudioCodec.Location = new System.Drawing.Point(0, 64);
			this.labelAudioCodec.Name = "labelAudioCodec";
			this.labelAudioCodec.Size = new System.Drawing.Size(59, 19);
			this.labelAudioCodec.TabIndex = 37;
			this.labelAudioCodec.Text = "Codec:";
			// 
			// tx_ACodec
			// 
			this.tx_ACodec.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_ACodec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_ACodec.Font = new System.Drawing.Font("Tahoma", 7F);
			this.tx_ACodec.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_ACodec.Location = new System.Drawing.Point(55, 64);
			this.tx_ACodec.Name = "tx_ACodec";
			this.tx_ACodec.ReadOnly = true;
			this.tx_ACodec.Size = new System.Drawing.Size(113, 19);
			this.tx_ACodec.TabIndex = 38;
			this.tx_ACodec.Text = "";
			// 
			// labelAudioChannels
			// 
			this.labelAudioChannels.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelAudioChannels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelAudioChannels.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelAudioChannels.ForeColor = System.Drawing.Color.Black;
			this.labelAudioChannels.Location = new System.Drawing.Point(167, 64);
			this.labelAudioChannels.Name = "labelAudioChannels";
			this.labelAudioChannels.Size = new System.Drawing.Size(107, 19);
			this.labelAudioChannels.TabIndex = 39;
			this.labelAudioChannels.Text = "Channels:";
			// 
			// tx_AChannels
			// 
			this.tx_AChannels.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_AChannels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_AChannels.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tx_AChannels.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_AChannels.Location = new System.Drawing.Point(273, 64);
			this.tx_AChannels.Name = "tx_AChannels";
			this.tx_AChannels.ReadOnly = true;
			this.tx_AChannels.Size = new System.Drawing.Size(106, 19);
			this.tx_AChannels.TabIndex = 40;
			this.tx_AChannels.Text = "";
			// 
			// labelAudioBitrate
			// 
			this.labelAudioBitrate.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelAudioBitrate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelAudioBitrate.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelAudioBitrate.ForeColor = System.Drawing.Color.Black;
			this.labelAudioBitrate.Location = new System.Drawing.Point(0, 82);
			this.labelAudioBitrate.Name = "labelAudioBitrate";
			this.labelAudioBitrate.Size = new System.Drawing.Size(59, 19);
			this.labelAudioBitrate.TabIndex = 41;
			this.labelAudioBitrate.Text = "Bitrate:";
			// 
			// tx_ABitrate
			// 
			this.tx_ABitrate.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_ABitrate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_ABitrate.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tx_ABitrate.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_ABitrate.Location = new System.Drawing.Point(55, 82);
			this.tx_ABitrate.Name = "tx_ABitrate";
			this.tx_ABitrate.ReadOnly = true;
			this.tx_ABitrate.Size = new System.Drawing.Size(113, 19);
			this.tx_ABitrate.TabIndex = 42;
			this.tx_ABitrate.Text = "";
			// 
			// labelAudioSampleRate
			// 
			this.labelAudioSampleRate.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(222)), ((System.Byte)(231)), ((System.Byte)(247)));
			this.labelAudioSampleRate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelAudioSampleRate.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelAudioSampleRate.ForeColor = System.Drawing.Color.Black;
			this.labelAudioSampleRate.Location = new System.Drawing.Point(167, 82);
			this.labelAudioSampleRate.Name = "labelAudioSampleRate";
			this.labelAudioSampleRate.Size = new System.Drawing.Size(107, 19);
			this.labelAudioSampleRate.TabIndex = 43;
			this.labelAudioSampleRate.Text = "Sample rate:";
			// 
			// tx_ASampleRate
			// 
			this.tx_ASampleRate.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.tx_ASampleRate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tx_ASampleRate.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tx_ASampleRate.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(68)), ((System.Byte)(69)), ((System.Byte)(151)));
			this.tx_ASampleRate.Location = new System.Drawing.Point(273, 82);
			this.tx_ASampleRate.Name = "tx_ASampleRate";
			this.tx_ASampleRate.ReadOnly = true;
			this.tx_ASampleRate.Size = new System.Drawing.Size(106, 19);
			this.tx_ASampleRate.TabIndex = 44;
			this.tx_ASampleRate.Text = "";
			// 
			// FormAviDetails
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(235)), ((System.Byte)(241)), ((System.Byte)(250)));
			this.ClientSize = new System.Drawing.Size(379, 103);
			this.Controls.Add(this.tx_ASampleRate);
			this.Controls.Add(this.tx_Length);
			this.Controls.Add(this.tx_Width);
			this.Controls.Add(this.tx_Height);
			this.Controls.Add(this.tx_VBitrate);
			this.Controls.Add(this.tx_VFps);
			this.Controls.Add(this.tx_VCodec);
			this.Controls.Add(this.tx_AChannels);
			this.Controls.Add(this.tx_ACodec);
			this.Controls.Add(this.tx_ABitrate);
			this.Controls.Add(this.labelVideoLength);
			this.Controls.Add(this.labelVideoWidth);
			this.Controls.Add(this.labelVideoHeight);
			this.Controls.Add(this.labelVideoBitrate);
			this.Controls.Add(this.labelVideoFPS);
			this.Controls.Add(this.labelVideoCodec);
			this.Controls.Add(this.labelVideoInfo);
			this.Controls.Add(this.labelAudioInfo);
			this.Controls.Add(this.labelAudioChannels);
			this.Controls.Add(this.labelAudioCodec);
			this.Controls.Add(this.labelAudioBitrate);
			this.Controls.Add(this.labelAudioSampleRate);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormAviDetails";
			this.ShowInTaskbar = false;
			this.Text = "FormAviDetails";
			this.ResumeLayout(false);

		}
		#endregion

		public void ApplySkin()
		{
			BackColor=eLePhantForm.Skin.GetColor("panelsBackColor");
			foreach (Control c in this.Controls)
			{
				if (c.GetType()==typeof(Label))
				{
					((Label)c).ForeColor=eLePhantForm.Skin.GetColor("SquaredLabelsForeColor");
					((Label)c).BackColor=eLePhantForm.Skin.GetColor("SquaredLabelsBackColor");
				}
				else if	(c.GetType()==typeof(TextBox))
				{
					((TextBox)c).ForeColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxForeColor");
					((TextBox)c).BackColor=eLePhantForm.Skin.GetColor("readOnlyTextBoxBackColor");
				}
			}
		}


	}
}
