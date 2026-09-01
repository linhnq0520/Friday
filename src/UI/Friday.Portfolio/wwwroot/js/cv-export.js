/**
 * CV PDF Export Service for Friday Portfolio
 * Generates and downloads an exact standard 2-page A4 PDF document
 */
window.exportCvToPdf = async function (fileName) {
    const resumeEl = document.querySelector('.print-resume-document');
    if (!resumeEl) return false;

    if (typeof html2pdf === 'undefined') {
        console.error('html2pdf library is not loaded');
        window.print();
        return false;
    }

    // 1. Tạo Overlay xoay vàng toàn màn hình phủ kín 100% (Solid Dark Background, z-index cao nhất)
    const loadingOverlay = document.createElement('div');
    loadingOverlay.id = 'cv-export-loading-overlay';
    loadingOverlay.style.cssText = 'position:fixed;top:0;left:0;right:0;bottom:0;width:100vw;height:100vh;z-index:2147483647;background:#071018;display:flex;align-items:center;justify-content:center;pointer-events:all;';
    
    const spinner = document.createElement('div');
    spinner.style.cssText = 'width:64px;height:64px;border:5px solid rgba(212,162,76,0.2);border-top:5px solid #d4a24c;border-radius:50%;animation:cv-gold-spin 0.85s linear infinite;box-shadow:0 0 24px rgba(212,162,76,0.25);';
    
    if (!document.getElementById('cv-gold-spin-style')) {
        const style = document.createElement('style');
        style.id = 'cv-gold-spin-style';
        style.textContent = '@keyframes cv-gold-spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }';
        document.head.appendChild(style);
    }

    loadingOverlay.appendChild(spinner);

    // 2. Khối render in ấn được đặt ẩn hoàn toàn ở z-index thấp phía sau Overlay
    const container = document.createElement('div');
    container.className = 'cv-pdf-render-wrapper';
    container.style.cssText = 'position:fixed;left:0;top:0;width:642px;min-width:642px;max-width:642px;background:#ffffff;color:#111111;padding:0;margin:0;z-index:100;pointer-events:none;box-sizing:border-box;';

    // Clone the resume content
    const clone = resumeEl.cloneNode(true);
    clone.classList.add('pdf-rendering-active');
    clone.style.setProperty('display', 'block', 'important');
    clone.style.setProperty('visibility', 'visible', 'important');
    clone.style.setProperty('opacity', '1', 'important');
    clone.style.setProperty('width', '642px', 'important');
    clone.style.setProperty('padding', '0', 'important');
    clone.style.setProperty('margin', '0', 'important');
    
    // Force all elements inside clone to have solid black text and transparent background
    clone.querySelectorAll('*').forEach(el => {
        el.style.setProperty('color', '#111111', 'important');
        el.style.setProperty('-webkit-text-fill-color', '#111111', 'important');
        if (el.tagName === 'H1' || el.tagName === 'H2' || el.tagName === 'H3' || el.classList.contains('print-sec-title') || el.classList.contains('print-name')) {
            el.style.setProperty('font-family', "Georgia, 'Times New Roman', Times, serif", 'important');
            el.style.setProperty('color', '#111111', 'important');
        }
    });

    container.appendChild(clone);

    // Append container first, then overlay ON TOP OF IT
    document.body.appendChild(container);
    document.body.appendChild(loadingOverlay);

    try {
        // Ensure all images are fully loaded
        const images = container.querySelectorAll('img');
        const imagePromises = Array.from(images).map(img => {
            if (img.complete) return Promise.resolve();
            return new Promise(resolve => {
                img.onload = resolve;
                img.onerror = resolve;
            });
        });
        await Promise.all(imagePromises);

        // Wait for fonts and layout to settle
        await new Promise(r => setTimeout(r, 200));

        const opt = {
            margin: [20, 20, 20, 20], // 20mm (2cm) margin on all 4 sides for ALL pages
            filename: fileName || 'Nguyen_Quoc_Linh_CV.pdf',
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: {
                scale: 2,
                useCORS: true,
                logging: false,
                letterRendering: true,
                backgroundColor: '#ffffff',
                width: 642,
                windowWidth: 642,
                x: 0,
                y: 0,
                scrollX: 0,
                scrollY: 0
            },
            jsPDF: {
                unit: 'mm',
                format: 'a4',
                orientation: 'portrait'
            },
            pagebreak: {
                mode: ['css', 'legacy'],
                avoid: ['.print-header-shell', '.print-sec-head', '.print-job', '.print-project', '.print-edu']
            }
        };

        await html2pdf().set(opt).from(clone).save();
        return true;
    } catch (err) {
        console.error('Error generating PDF with html2pdf:', err);
        return false;
    } finally {
        if (document.body.contains(container)) {
            document.body.removeChild(container);
        }
        if (document.body.contains(loadingOverlay)) {
            document.body.removeChild(loadingOverlay);
        }
    }
};

window.getCvPdfDataUri = async function () {
    const resumeEl = document.querySelector('.print-resume-document');
    if (!resumeEl || typeof html2pdf === 'undefined') return null;

    const container = document.createElement('div');
    container.className = 'cv-pdf-render-wrapper';
    container.style.position = 'fixed';
    container.style.left = '0';
    container.style.top = '0';
    container.style.width = '642px';
    container.style.minWidth = '642px';
    container.style.maxWidth = '642px';
    container.style.backgroundColor = '#ffffff';
    container.style.color = '#111111';
    container.style.padding = '0';
    container.style.margin = '0';
    container.style.zIndex = '99998';
    container.style.pointerEvents = 'none';
    container.style.boxSizing = 'border-box';

    const clone = resumeEl.cloneNode(true);
    clone.style.display = 'block';
    clone.style.visibility = 'visible';
    clone.style.opacity = '1';
    clone.classList.add('pdf-rendering-active');
    clone.style.setProperty('width', '642px', 'important');
    clone.style.setProperty('padding', '0', 'important');
    clone.style.setProperty('margin', '0', 'important');

    clone.querySelectorAll('*').forEach(el => {
        el.style.setProperty('color', '#111111', 'important');
        el.style.setProperty('-webkit-text-fill-color', '#111111', 'important');
        if (el.tagName === 'H1' || el.tagName === 'H2' || el.tagName === 'H3' || el.classList.contains('print-sec-title') || el.classList.contains('print-name')) {
            el.style.setProperty('font-family', "Georgia, 'Times New Roman', Times, serif", 'important');
            el.style.setProperty('color', '#111111', 'important');
        }
    });

    container.appendChild(clone);
    document.body.appendChild(container);

    try {
        const images = container.querySelectorAll('img');
        const imagePromises = Array.from(images).map(img => {
            if (img.complete) return Promise.resolve();
            return new Promise(resolve => {
                img.onload = resolve;
                img.onerror = resolve;
            });
        });
        await Promise.all(imagePromises);
        await new Promise(r => setTimeout(r, 200));

        const opt = {
            margin: [20, 20, 20, 20],
            filename: 'Nguyen_Quoc_Linh_CV.pdf',
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: {
                scale: 2,
                useCORS: true,
                logging: false,
                letterRendering: true,
                backgroundColor: '#ffffff',
                width: 642,
                windowWidth: 642,
                x: 0,
                y: 0,
                scrollX: 0,
                scrollY: 0
            },
            jsPDF: {
                unit: 'mm',
                format: 'a4',
                orientation: 'portrait'
            },
            pagebreak: {
                mode: ['css', 'legacy'],
                avoid: ['.print-header-shell', '.print-sec-head', '.print-job', '.print-project', '.print-edu']
            }
        };

        const dataUri = await html2pdf().set(opt).from(clone).outputPdf('datauristring');
        return dataUri;
    } catch (err) {
        console.error('Error generating PDF dataUri:', err);
        return null;
    } finally {
        if (document.body.contains(container)) {
            document.body.removeChild(container);
        }
    }
};
