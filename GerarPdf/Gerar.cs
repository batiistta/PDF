using Gerarpdf;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerarPdf
{
    public class Gerar
    {
        public void Criar()
        {
            MemoryStream ms = new MemoryStream();
            string folderName = "Comprovantes";
            string fileName = "teste3.pdf";
            string pathProject = @"C:\Users\gabriel.batista\Documents\Projeto\Pessoal\PDF\PDF\bin\Debug\net6.0-windows";        

            string pathLogo = @"C:\Users\gabriel.batista\Documents\Projeto\Pessoal\PDF\PDF\bin\Debug\net6.0-windows\Images\logo1.png";         


            string outputPath = System.IO.Path.Combine(pathProject, folderName, fileName);
            PdfWriter pw = new PdfWriter(outputPath);
            PdfDocument pdfDocument = new PdfDocument(pw);
            Document doc = new Document(pdfDocument, PageSize.A4);
            doc.SetMargins(75, 35, 75, 35);

            Image img = new Image(ImageDataFactory.Create(pathLogo));
           
            pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler1(img));
            pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler1(img));
            pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler1());
            //pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new BackgroundColorHandler());

            Table table = new Table(1).UseAllAvailableWidth();
            Cell cell = new Cell().Add(new Paragraph("Produtos").SetFontSize(14))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(Border.NO_BORDER);
            table.AddCell(cell);
            cell = new Cell().Add(new Paragraph("Produtos existentes"))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(Border.NO_BORDER)                ;
            table.AddCell(cell);

            doc.Add(table);

            Style styleCell = new Style()
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER);

            Table table2 = new Table(4).UseAllAvailableWidth();
            Cell cell2 = new Cell(2, 1).Add(new Paragraph("#"));
            table2.AddHeaderCell(cell2.AddStyle(styleCell));
            cell2 = new Cell(1, 2).Add(new Paragraph("Produto"));
            table2.AddHeaderCell(cell2.AddStyle(styleCell));           
            cell2 = new Cell(2, 1).Add(new Paragraph("Unidades existentes"));
            table2.AddHeaderCell(cell2.AddStyle(styleCell));
            cell2 = new Cell().Add(new Paragraph("Nome"));
            table2.AddHeaderCell(cell2.AddStyle(styleCell));
            cell2 = new Cell().Add(new Paragraph("Preço Unitário"));
            table2.AddHeaderCell(cell2.AddStyle(styleCell));


            Random random = new Random();
            List<Produto> produtos = new List<Produto>();
            // Função para gerar um nome aleatório
            string GerarNomeAleatorio()
            {
                string[] nomes = { "fone", "teclado", "mouse", "monitor", "impressora", "notebook", "celular", "tablet", "câmera", "caixa de som" };
                int index = random.Next(nomes.Length);
                return nomes[index];
            }

            for (int i = 0; i < 10; i++)
            {
                Produto produto = new Produto();
                produto.Nome = GerarNomeAleatorio();
                produto.Quantidade = random.Next(1, 100);
                produto.Preco = (decimal)random.NextDouble() * 100;

                produtos.Add(produto);
            }
            int x = 0;
            foreach (var item in produtos)
            {
                x++;
                cell2 = new Cell().Add(new Paragraph(x.ToString()));
                table2.AddCell(cell2.SetBackgroundColor(ColorConstants.GREEN));
                cell2 = new Cell().Add(new Paragraph(item.Nome));
                table2.AddCell(cell2.SetBackgroundColor(ColorConstants.ORANGE));
                cell2 = new Cell().Add(new Paragraph(item.Preco.ToString("c")));
                table2.AddCell(cell2.SetBackgroundColor(ColorConstants.YELLOW));

                if (item.Quantidade < 10)
                {
                    cell2 = new Cell().Add(new Paragraph(item.Quantidade.ToString()));
                    table2.AddCell(cell2.SetBackgroundColor(ColorConstants.RED));
                }
                else
                {
                    cell2 = new Cell().Add(new Paragraph(item.Quantidade.ToString()));
                    table2.AddCell(cell2);
                }

            }


            doc.Add(table2);

            doc.Close();



            /*byte[] bytesStream = ms.ToArray();
            ms = new MemoryStream();
            ms.Write(bytesStream, 0, bytesStream.Length);
            ms.Position = 0;

            return new FileStreamResult(ms, "arquivo/pdf");*/
        }
    }

    public class HeaderEventHandler1 : IEventHandler
    {
        Image Img;
        public HeaderEventHandler1(Image img)
        {
            Img = img;
        }
        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
            PdfDocument pdfDoc = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();

            PdfCanvas canvas1 = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
            Rectangle rootArea = new Rectangle(35, page.GetPageSize().GetTop() - 75, page.GetPageSize().GetWidth()-62, 55);
            new Canvas(canvas1, pdfDoc, rootArea)
                .Add(getTable(docEvent));

            /*Rectangle rootArea = new Rectangle(35, page.GetPageSize().GetTop() - 70, page.GetPageSize().GetRight() - 70, 50);

            Canvas canvas = new Canvas(page, rootArea);
            canvas
                .Add(getTable(docEvent))
                .ShowTextAligned("Cabeçalho da página", 10, 0, TextAlignment.CENTER)
                .ShowTextAligned("Rodapé da página", 10, 10, TextAlignment.CENTER)
                .ShowTextAligned("Texto agregado", 612, 0, TextAlignment.RIGHT)
                .Close();*/
        }

        private IBlockElement getTable(PdfDocumentEvent docEvent)
        {
            float[] cellWidth = { 282, 40f };
            Table tableEvent = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

            Style styleCell = new Style()
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 2));
            Style styleText = new Style()
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFontSize(10f);

            Cell cell = new Cell().Add(Img.SetAutoScale(true));

            tableEvent.AddCell(cell
                .AddStyle(styleCell)
                .SetTextAlignment(TextAlignment.LEFT));


            Cell cell1 = new Cell().Add(Img.SetAutoScale(true));
            tableEvent.AddCell(cell1
               .AddStyle(styleCell)
               .SetTextAlignment(TextAlignment.RIGHT));

            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);

            //cell = new Cell()
            //    .Add(new Paragraph("Reporte Diário\n").SetFont(bold))
            //    .Add(new Paragraph("Recursos Materiais\n").SetFont(bold))
            //    .Add(new Paragraph("Data de emissão: " + DateTime.Now.ToShortDateString()))
            //    .AddStyle(styleText).AddStyle(styleCell);

            //tableEvent.AddCell(cell);

            return tableEvent;
        }


    }

    public class FooterEventHandler1 : IEventHandler
    {
        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
            PdfDocument pdfdoc = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();
            PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfdoc);
            new Canvas(canvas, pdfdoc, new Rectangle(36, 20, page.GetPageSize().GetWidth() - 70, 50))
                .Add(getTable(docEvent));
        }

        public Table getTable(PdfDocumentEvent docEvent)
        {
            float[] cellWidth = { 92f, 8f };
            Table tableEvent = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

            int pageNum = docEvent.GetDocument().GetPageNumber(docEvent.GetPage());

            Style styleCell = new Style()
                .SetPadding(5)
                .SetBorder(Border.NO_BORDER)
                .SetBorderTop(new SolidBorder(ColorConstants.BLACK, 2));

            Cell cell = new Cell().Add(new Paragraph(DateTime.Now.ToLongDateString()));
            tableEvent.AddCell(cell
                .AddStyle(styleCell)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFontColor(ColorConstants.LIGHT_GRAY));
            cell = new Cell().Add(new Paragraph(pageNum.ToString()));
            cell.AddStyle(styleCell)
                .SetBackgroundColor(ColorConstants.BLACK)
                .SetFontColor(ColorConstants.WHITE)
                .SetTextAlignment(TextAlignment.CENTER);
            tableEvent.AddCell(cell);

            return tableEvent;
        }
    }
   


}
