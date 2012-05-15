using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using FM;

namespace ARPPoisoningProtection
{
    public partial class ArpPoisoningProtection : UserControl
    {
        ARPPP saap;
        SerializableDictionary<IPAddress, byte[]> cache = new SerializableDictionary<IPAddress, byte[]>();

        public ArpPoisoningProtection(ARPPP saap)
        {
            if (null != saap)
            {
                this.saap = saap;
                cache = saap.GetCache();
                saap.UpdatedArpCache += new System.Threading.ThreadStart(saap_UpdatedArpCache);
                InitializeComponent();
                saap_UpdatedArpCache();
            }
        }

        void saap_UpdatedArpCache()
        {
            if (listBox1.InvokeRequired)
            {
                cache = saap.GetCache();
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(saap_UpdatedArpCache);
                listBox1.Invoke(ts);
            }
            else
            {
                listBox1.Items.Clear();
                foreach (KeyValuePair<IPAddress, byte[]> i in cache)
                {
                    listBox1.Items.Add(BitConverter.ToString(i.Value).Replace("-", "") + " -> " + i.Key.ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxSave_CheckedChanged(object sender, EventArgs e)
        {
            saap.data.Save = checkBoxSave.Checked;
        }

        private void checkBoxLogUnsolicited_CheckedChanged(object sender, EventArgs e)
        {
            saap.data.LogUnsolic = checkBoxLogUnsolicited.Checked;
        }

        private void checkBoxLogPoisoning_CheckedChanged(object sender, EventArgs e)
        {
            saap.data.LogAttacks = checkBoxLogPoisoning.Checked;
        }

        private void ArpPoisoningProtection_Load(object sender, EventArgs e)
        {
            checkBoxLogPoisoning.Checked = saap.data.LogAttacks;
            checkBoxLogUnsolicited.Checked = saap.data.LogUnsolic;
            checkBoxSave.Checked = saap.data.Save;
            checkBoxRectify.Checked = saap.data.RectifyAttacks;
            switch (LanguageConfig.GetCurrentLanguage())
            {
                case LanguageConfig.Language.NONE:
                case LanguageConfig.Language.ENGLISH:
                    checkBoxSave.Text = "Save ARP Cache";
                    checkBoxLogUnsolicited.Text = "Log Unsolicited";
                    checkBoxLogPoisoning.Text = "Log Attacks";
                    button1.Text = "Remove Entry";
                    button2.Text = "Clear Cache";
                    checkBoxRectify.Text = "Rectify Attacks";
                    break;
                case LanguageConfig.Language.PORTUGUESE:
                    checkBoxSave.Text = "Salve Cache ARP";
                    checkBoxLogUnsolicited.Text = "Entrar Unsolicited";
                    checkBoxLogPoisoning.Text = "Entrar Ataques";
                    button1.Text = "remover Entrada";
                    button2.Text = "Limpar Cache";
                    checkBoxRectify.Text = "Eliminar os ataques";
                    break;
                case LanguageConfig.Language.CHINESE:
                    checkBoxSave.Text = "保存ARP缓存";
                    checkBoxLogUnsolicited.Text = "登录未经请求";
                    checkBoxLogPoisoning.Text = "登录攻击";
                    button1.Text = "删除条目";
                    button2.Text = "清除缓存";
                    checkBoxRectify.Text = "纠正攻击";
                    break;
                case LanguageConfig.Language.GERMAN:
                    checkBoxSave.Text = "Save ARP Cache";
                    checkBoxLogUnsolicited.Text = "Log Initiativbewerbung";
                    checkBoxLogPoisoning.Text = "Log Attacks";
                    button1.Text = "Eintrag entfernen";
                    button2.Text = "Cache löschen";
                    checkBoxRectify.Text = "beheben Sie Angriffe";
                    break;
                case LanguageConfig.Language.RUSSIAN:
                    checkBoxSave.Text = "Сохранить ARP кэш";
                    checkBoxLogUnsolicited.Text = "Вход Незапрошенные";
                    checkBoxLogPoisoning.Text = "Вход атак";
                    button1.Text = "Удалить Вступление";
                    button2.Text = "Очистить кэш";
                    checkBoxRectify.Text = "Устранение атак";
                    break;
                case LanguageConfig.Language.SPANISH:
                    checkBoxSave.Text = "Guardar la caché ARP";
                    checkBoxLogUnsolicited.Text = "registro no solicitados";
                    checkBoxLogPoisoning.Text = "Los ataques de registro";
                    button1.Text = "eliminar entrada";
                    button2.Text = "Borrar la caché";
                    checkBoxRectify.Text = "rectificar los ataques";
                    break;
            }
            saap_UpdatedArpCache();
        }

        private void checkBoxRectify_CheckedChanged(object sender, EventArgs e)
        {
            saap.data.RectifyAttacks = checkBoxRectify.Checked;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedItem != null)
                {
                    string i = (string)listBox1.SelectedItem;
                    IPAddress ip = IPAddress.Parse(i.Split(' ')[2]);
                    cache.Remove(ip);
                    saap.UpdateCache(cache);
                    cache = saap.GetCache();
                    saap_UpdatedArpCache();
                }
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cache.Clear();
            saap.UpdateCache(cache);
            cache = saap.GetCache();
            saap_UpdatedArpCache();
        }
    }
}
