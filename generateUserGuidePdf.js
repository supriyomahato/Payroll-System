#!/usr/bin/env node

const fs = require('fs');
const path = require('path');
const PDFDocument = require('pdfkit');

function generatePDF() {
  const mdFilePath = path.join(__dirname, 'USER_GUIDE.md');
  const pdfFilePath = path.join(__dirname, 'USER_GUIDE.pdf');

  // Read markdown file
  const mdContent = fs.readFileSync(mdFilePath, 'utf8');

  try {
    console.log('🚀 Starting PDF generation...');

    // Create PDF document
    const doc = new PDFDocument({
      margin: 50,
      bufferPages: true
    });

    // Pipe to file
    const stream = fs.createWriteStream(pdfFilePath);
    doc.pipe(stream);

    // Set document properties
    const boldFont = 'Helvetica-Bold';
    const normalFont = 'Helvetica';
    const codeFont = 'Courier';

    // Split content into lines and process
    const lines = mdContent.split('\n');
    let y = doc.y;

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];

      // Check for page overflow
      if (y > doc.page.height - 60) {
        doc.addPage();
        y = 50;
      }

      if (line.startsWith('# ')) {
        // Main title
        doc.font(boldFont, 24).text(line.substring(2), { lineGap: 10 });
        doc.moveTo(50, doc.y).lineTo(550, doc.y).stroke('#1e40af');
        doc.moveDown(0.5);
        y = doc.y;
      } else if (line.startsWith('## ')) {
        // Heading 2
        doc.moveDown(0.3);
        doc.font(boldFont, 16).fillColor('#1e40af').text(line.substring(3), { lineGap: 8 });
        doc.fillColor('#000');
        doc.moveDown(0.2);
        y = doc.y;
      } else if (line.startsWith('### ')) {
        // Heading 3
        doc.moveDown(0.2);
        doc.font(boldFont, 13).fillColor('#2563eb').text(line.substring(4), { lineGap: 6 });
        doc.fillColor('#000');
        doc.moveDown(0.1);
        y = doc.y;
      } else if (line.startsWith('#### ')) {
        // Heading 4
        doc.font(boldFont, 11).fillColor('#2563eb').text(line.substring(5), { lineGap: 5 });
        doc.fillColor('#000');
        y = doc.y;
      } else if (line.startsWith('- ')) {
        // Bullet point
        doc.font(normalFont, 10);
        const text = line.substring(2);
        doc.text('• ' + text, { indent: 20, width: 450, align: 'left', lineGap: 3 });
        y = doc.y;
      } else if (line.startsWith('| ')) {
        // Skip table headers - simplify for PDF
        continue;
      } else if (line.match(/^[0-9]+\. /)) {
        // Numbered list
        doc.font(normalFont, 10);
        doc.text(line, { indent: 20, width: 450, align: 'left', lineGap: 3 });
        y = doc.y;
      } else if (line === '' || line === '---') {
        // Empty line or divider
        if (line === '---') {
          doc.moveTo(50, doc.y).lineTo(550, doc.y + 2).stroke('#ddd');
          doc.moveDown(0.5);
        } else {
          doc.moveDown(0.3);
        }
        y = doc.y;
      } else if (line.trim().length > 0) {
        // Regular paragraph
        doc.font(normalFont, 10);
        // Remove markdown formatting
        let cleanLine = line
          .replace(/\*\*(.*?)\*\*/g, '$1')
          .replace(/\*(.*?)\*/g, '$1')
          .replace(/\`(.*?)\`/g, '$1')
          .replace(/\[(.*?)\]\(.*?\)/g, '$1');

        doc.text(cleanLine, { width: 500, align: 'left', lineGap: 4 });
        y = doc.y;
      }
    }

    // Add footer with page numbers
    const pageCount = doc.bufferedPageRange().count;
    for (let i = 0; i < pageCount; i++) {
      doc.switchToPage(i);
      doc.font(normalFont, 8)
        .fillColor('#999')
        .text(
          `Page ${i + 1} of ${pageCount}`,
          50,
          doc.page.height - 30,
          { align: 'center' }
        );
    }

    // End document
    doc.end();

    // Handle stream finish
    stream.on('finish', () => {
      const fileSize = fs.statSync(pdfFilePath).size / 1024; // in KB
      console.log(`✅ PDF generated successfully!`);
      console.log(`📄 File: ${pdfFilePath}`);
      console.log(`📊 Size: ${fileSize.toFixed(2)} KB`);
      console.log(`📖 Pages: ${pageCount}`);
      console.log(`🎉 User Guide is ready for distribution!`);
    });

    stream.on('error', (error) => {
      console.error('❌ Error writing PDF:', error);
      process.exit(1);
    });

  } catch (error) {
    console.error('❌ Error generating PDF:', error);
    process.exit(1);
  }
}

generatePDF();
