import MyCKEditor from "@/components/ckeditor";
import { apiAddLeadFeedback, apiGetLeadFeedback, apiGetSmDosOptions, apiGetTableOptions, apiUpdateLeadFeedback } from "@/services/contact";
import { apiSalesOptions } from "@/services/user";
import { SOURCES } from "@/utils/constants";
import { PlusOutlined } from "@ant-design/icons";
import { DrawerForm, ModalFormProps, ProFormDigit, ProFormInstance, ProFormSelect, ProFormText, ProFormTextArea, ProFormTimePicker } from "@ant-design/pro-components";
import { useAccess } from "@umijs/max";
import { Button, Col, Divider, Input, InputRef, message, Row, Space } from "antd";
import dayjs from "dayjs";
import { useEffect, useRef, useState } from "react";

type Props = ModalFormProps & {
    readonly?: boolean;
    reload: any;
    eventDate: string;
    eventTime: string;
}

const LeadFeedback: React.FC<Props> = (props) => {
    let index = 0;
    const formRef = useRef<ProFormInstance>();
    const [tables, setTables] = useState<any>([]);
    const access = useAccess();
    const [loading, setLoading] = useState<boolean>(true);
    const [items, setItems] = useState(SOURCES);
    const [name, setName] = useState('');
    const inputRef = useRef<InputRef>(null);
    const onNameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setName(event.target.value);
    };

    const addItem = (e: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>) => {
        e.preventDefault();
        setItems([...items, name || `New item ${index++}`]);
        setName('');
        setTimeout(() => {
            inputRef.current?.focus();
        }, 0);
    };

    useEffect(() => {
        if (props.open) {
            apiGetTableOptions({ floor: 'Lầu 1', eventDate: props.eventDate, eventTime: props.eventTime }).then(response => setTables(response));
        }
    }, [props.open, props.eventDate]);

    useEffect(() => {
        if (props.id && props.open) {
            setLoading(true);
            apiGetLeadFeedback(props.id).then(response => {
                formRef.current?.setFields([
                    {
                        name: 'to',
                        value: response.to
                    },
                    {
                        name: 'financialSituation',
                        value: response.financialSituation
                    },
                    {
                        name: 'interestLevel',
                        value: response.interestLevel
                    },
                    {
                        name: 'rejectReason',
                        value: response.rejectReason
                    },
                    {
                        name: 'id',
                        value: response.id
                    },
                    {
                        name: 'tableId',
                        value: response.tableId
                    },
                    {
                        name: 'source',
                        value: response.source
                    },
                    {
                        name: 'jobTitle',
                        value: response.jobTitle
                    },
                    {
                        name: 'contractCode',
                        value: response.contractCode
                    },
                    {
                        name: 'contractCode2',
                        value: response.contractCode2
                    },
                    {
                        name: 'contractAmount',
                        value: response.contractAmount
                    },
                    {
                        name: 'amountPaid',
                        value: response.amountPaid
                    },
                    {
                        name: 'tableStatus',
                        value: response.tableStatus
                    },
                    {
                        name: 'age',
                        value: response.age
                    },
                    {
                        name: 'evidence',
                        value: response.evidence
                    },
                    {
                        name: 'sellerId',
                        value: response.sellerId
                    },
                    {
                        name: 'checkinTime',
                        value: response.checkinTime ? dayjs(response.checkinTime, 'hh:mm:ss') : null
                    },
                    {
                        name: 'voucher',
                        value: response.voucher
                    },
                    {
                        name: 'isOnsiteTesting',
                        value: response.isOnsiteTesting
                    },
                    {
                        name: 'checkoutTime',
                        value: response.checkoutTime ? dayjs(response.checkoutTime, 'hh:mm:ss') : null
                    }
                ]);
                const contractAmount = formRef.current?.getFieldValue('contractAmount');
                const amountPaid = formRef.current?.getFieldValue('amountPaid');
                if (contractAmount && amountPaid) {
                    const percent = amountPaid / contractAmount * 100;
                    formRef.current?.setFieldValue('amountPercent', percent.toFixed(2));
                }
                setLoading(false);
            })
        }
    }, [props.id, props.open]);

    return (
        <DrawerForm
            width={1000}
            formRef={formRef}
            modalProps={{
                centered: true
            }}
            {...props} title="Feedback Form" onFinish={async (values) => {
                values.leadId = props.id;
                values.eventTime = props.eventTime;
                values.eventDate = props.eventDate;
                if (values.id) {
                    await apiUpdateLeadFeedback(values);
                } else {
                    await apiAddLeadFeedback(values);
                }
                message.success('Thành công!');
                props.onOpenChange?.(false);
                props.reload();
            }}
            disabled={access.telesale || access.telesaleManager}
        >
            <ProFormText hidden name="id" />
            <Row gutter={16}>
                <Col md={8} xs={12}>
                    <ProFormSelect request={apiGetSmDosOptions} name="to" label="Người TO" showSearch disabled={access.sales} />
                </Col>
                <Col md={8} xs={12}>
                    <ProFormSelect request={apiSalesOptions} name="sellerId" label="Trợ lý tiếp khách" showSearch disabled={!access.event} />
                </Col>
                <Col md={8} xs={12}>
                    <ProFormSelect options={[
                        'Thấp (không có tiết kiệm)',
                        'Trung bình (có tích lũy dưới 50M)',
                        'Khá (có tích lũy dưới 200M)',
                        'Tốt (có tích lũy trên 200M)'
                    ]} name="financialSituation" label="Tình hình tài chính" />
                </Col>
                <Col md={6} xs={12}>
                    <ProFormSelect options={['1', '2', '3', '4', '5', '6', '7', '8', '9', '10']} name="interestLevel" label="Mức độ quan tâm" />
                </Col>
                <Col md={10} xs={24}>
                    <ProFormText label="Nghề nghiệp" name="jobTitle" />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormDigit label="Độ tuổi" name="age" />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormSelect label="Xét nghiệm tại chỗ" name="isOnsiteTesting" initialValue={false} options={[
                        {
                            label: 'Không',
                            value: false as any
                        },
                        {
                            label: 'Có',
                            value: true as any
                        }
                    ]} />
                </Col>
            </Row>
            <ProFormTextArea label="Lý do từ chối" name="rejectReason" />
            <MyCKEditor label="Chứng từ" name="evidence" loading={loading} className="w-full" disabled={access.telesale || access.telesaleManager} />
            <Row gutter={16}>
                {/* <Col md={3}>
                    <ProFormSelect name="floor" label="Khu vực" request={apiGetFloorOptions} disabled={!access.event} fieldProps={{
                        onChange: (value) => setFloor(value)
                    }} />
                </Col> */}
                <Col md={4} xs={12}>
                    <ProFormSelect name="tableId" label="Bàn" options={tables} showSearch disabled={!access.event} />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormSelect name="tableStatus" label="Trạng thái" disabled={!access.event} options={[
                        'Single Nam',
                        'Single Nữ',
                        'Couple',
                        'Two Single Nam',
                        'Two Single Nữ',
                        'Outside',
                        'Sendhome'
                    ]} />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormTimePicker name="checkinTime" label="Giờ Check-in" width="lg" disabled={!access.event} />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormTimePicker name="checkoutTime" label="Giờ Check-out" width="lg" disabled={!access.event} />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormSelect name="source" label="Nguồn" rules={[
                        {
                            required: access.event
                        }
                    ]} disabled={!access.event} fieldProps={{
                        dropdownRender: (menu) => (
                            <>
                                {menu}
                                <Divider style={{ margin: '8px 0' }} />
                                <Space style={{ padding: '0 8px 4px' }}>
                                    <Input
                                        placeholder="Chọn nguồn"
                                        ref={inputRef}
                                        value={name}
                                        onChange={onNameChange}
                                        onKeyDown={(e) => e.stopPropagation()}
                                    />
                                    <Button type="text" icon={<PlusOutlined />} onClick={addItem}>

                                    </Button>
                                </Space>
                            </>
                        )
                    }}
                        options={items.map((item) => ({ label: item, value: item }))} />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormText name="voucher" label="Voucher" disabled={!access.event} />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormText name="contractCode" label="Mã hợp đồng" disabled={!access.event} />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormText name="contractCode2" label="Hợp đồng 2" disabled={!access.event} tooltip="Nhập mã hợp đồng thứ 2 nếu có" />
                </Col>
                <Col md={6} xs={12}>
                    <ProFormDigit name="contractAmount" label="GTHĐ" disabled={!access.event} fieldProps={{
                        onChange: (value) => {
                            const amountPaid = formRef.current?.getFieldValue('amountPaid');
                            if (!amountPaid) return;
                            if (!value || value < 0) return;
                            const percent = amountPaid / value * 100;
                            formRef.current?.setFieldValue('amountPercent', percent.toFixed(2));
                            console.log(value)
                        },
                        formatter: (value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ','),
                        parser: (value) => value?.replace(/(,*)/g, '') as unknown as number
                    }} />
                </Col>
                <Col md={6} xs={12}>
                    <ProFormDigit name="amountPaid" disabled={!access.event} label="Số tiền đã thanh toán:" fieldProps={{
                        onChange: (value) => {
                            const contractAmount = formRef.current?.getFieldValue('contractAmount');
                            if (!contractAmount) return;
                            if (!value || value < 0) return;
                            const percent = value / contractAmount * 100;
                            formRef.current?.setFieldValue('amountPercent', percent.toFixed(2));
                        },
                        formatter: (value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ','),
                        parser: (value) => value?.replace(/(,*)/g, '') as unknown as number
                    }} />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormText name="amountPercent" readonly label="Tỷ lệ %" fieldProps={{
                        suffix: '%'
                    }} />
                </Col>
            </Row>
        </DrawerForm>
    )
}

export default LeadFeedback;